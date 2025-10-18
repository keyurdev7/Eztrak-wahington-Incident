// Repositories/Common/IncidentRoleService.cs
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
using System.Linq.Expressions;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class IncidentRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<IncidentRole, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IIncidentRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public IncidentRoleService(
            ApplicationDbContext db,
            ILogger<IncidentRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
        ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<IncidentRoleModifyViewModel>> GetAllIncidentRoles()
        {
            List<IncidentRoleModifyViewModel> list = new();
            try
            {
                var roles = await _db.IncidentRoles.Where(p => !p.IsDeleted).ToListAsync();
                foreach (var r in roles)
                {
                    list.Add(new IncidentRoleModifyViewModel()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllIncidentRoles.");
                return new List<IncidentRoleModifyViewModel>();
            }
            return list;
        }

        public async Task<long> SaveIncidentRole(IncidentRoleModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = new IncidentRole
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description ?? string.Empty
                };

                await _db.IncidentRoles.AddAsync(entity);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return entity.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncidentRole.");
                return 0;
            }
        }

        public async Task<long> UpdateIncidentRole(IncidentRoleModifyViewModel viewModel)
        {
            try
            {
                var entity = await _db.IncidentRoles.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();
                if (entity == null)
                {
                    return await SaveIncidentRole(viewModel);
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                entity.Name = viewModel.Name;
                entity.Description = viewModel.Description ?? string.Empty;

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

                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateIncidentRole.");
                return 0;
            }
        }

        public async Task<IncidentRoleModifyViewModel> GetIncidentRoleById(long Id)
        {
            var vm = new IncidentRoleModifyViewModel();
            try
            {
                var entity = await _db.IncidentRoles.FirstOrDefaultAsync(p => p.Id == Id);
                if (entity == null)
                    return new IncidentRoleModifyViewModel();

                vm.Id = entity.Id;
                vm.Name = entity.Name;
                vm.Description = entity.Description;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentRoleById.");
                return new IncidentRoleModifyViewModel();
            }

            return vm;
        }

        public async Task<long> DeleteIncidentRole(long Id)
        {
            try
            {
                var entity = await _db.IncidentRoles.Where(p => p.Id == Id).FirstOrDefaultAsync();
                if (entity == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();

                entity.IsDeleted = true;

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

                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteIncidentRole.");
                return 0;
            }
        }
    }
}
