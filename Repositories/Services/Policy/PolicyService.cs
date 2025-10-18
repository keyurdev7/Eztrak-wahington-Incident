using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class PolicyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<Policy, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IPolicyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PolicyService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public PolicyService(
            ApplicationDbContext db,
            ILogger<PolicyService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        //public override async Task<Expression<Func<Policy, bool>>> SetQueryFilter(IBaseSearchModel filters)
        //{
        //    var searchFilters = filters as PolicySearchViewModel;

        //    return x =>
        //                (
        //                    (
        //                        string.IsNullOrEmpty(searchFilters.Search.value)
        //                        ||
        //                        x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                        ||
        //                        (!string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(searchFilters.Search.value.ToLower()))
        //                    )
        //                )
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Description) || (!string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(searchFilters.Description.ToLower())))
        //                ;
        //}

        // -- CRUD methods similar to EventTypeService --

        public async Task<List<PolicyModifyViewModel>> GetAllPolicies()
        {
            var policies = new List<PolicyModifyViewModel>();
            try
            {
                var list = await _db.Policies.Where(p => !p.IsDeleted).ToListAsync();
                foreach (var p in list)
                {
                    policies.Add(new PolicyModifyViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        PolicySteps = p.PolicySteps?.Split(',').ToList() ?? new List<string>()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllPolicies.");
                return new List<PolicyModifyViewModel>();
            }

            return policies;
        }

        public async Task<long> SavePolicy(PolicyModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var policy = new Policy
                {
                    Name = viewModel.Name,
                    Description = "Description",
                    PolicySteps = viewModel.PolicySteps != null && viewModel.PolicySteps.Any()
                        ? string.Join(", ", viewModel.PolicySteps.Select(s => s.Replace(",", " ").Trim()))
                        : null
                };

                await _db.Policies.AddAsync(policy);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return policy.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SavePolicy.");
                return 0;
            }
        }

        public async Task<long> UpdatePolicy(PolicyModifyViewModel viewModel)
        {
            try
            {
                var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == viewModel.Id);

                // If not found, treat it as create and return created id
                if (policy == null)
                {
                    return await SavePolicy(viewModel);
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                policy.Name = viewModel.Name;
                //policy.Description = viewModel.Description;
                policy.PolicySteps = viewModel.PolicySteps != null && viewModel.PolicySteps.Any()
                    ? string.Join(", ", viewModel.PolicySteps.Select(s => s.Replace(",", " ").Trim()))
                    : null;

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {   
                    await transaction.RollbackAsync();
                    throw;
                }

                return policy.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdatePolicy.");
                return 0;
            }
        }

        public async Task<PolicyModifyViewModel> GetPolicyById(long id)
        {
            var vm = new PolicyModifyViewModel();
            try
            {
                var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
                if (policy == null)
                    return new PolicyModifyViewModel();

                vm.Id = policy.Id;
                vm.Name = policy.Name;
                vm.Description = policy.Description;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetPolicyById.");
                return new PolicyModifyViewModel();
            }

            return vm;
        }

        public async Task<long> DeletePolicy(long id)
        {
            try
            {
                var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == id);
                if (policy == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();
                policy.IsDeleted = true;

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return policy.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeletePolicy.");
                return 0;
            }
           
        }
        
        public async Task<long> AddPolicySteps(long policyId, IEnumerable<string> steps)
        {
            if (policyId <= 0 || steps == null || !steps.Any())
                return 0;

            // sanitize incoming steps (trim, drop empties)
            var incoming = steps
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (!incoming.Any()) return 0;

            try
            {
                var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted);
                if (policy == null) return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();

                var existing = policy.PolicySteps ?? string.Empty;
                var existingList = existing
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                // optionally sanitize incoming steps to avoid extra commas:
                var sanitizedIncoming = incoming.Select(s => s.Replace(",", " ").Trim()).ToList();

                // append incoming
                existingList.AddRange(sanitizedIncoming);

                // You might want to remove duplicates:
                // existingList = existingList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                policy.PolicySteps = string.Join(", ", existingList);

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return policy.Id;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error AddPolicySteps.");
                return 0;
            }
        }

    }
}
