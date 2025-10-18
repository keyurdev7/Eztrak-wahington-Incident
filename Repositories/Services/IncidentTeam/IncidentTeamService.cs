using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Enums; // ActiveStatus enum
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class IncidentTeamService<CreateViewModel, UpdateViewModel, DetailViewModel>
     : BaseService<IncidentTeam, CreateViewModel, UpdateViewModel, DetailViewModel>,
       IIncidentTeamService<CreateViewModel, UpdateViewModel, DetailViewModel>
     where DetailViewModel : class, IBaseCrudViewModel, new()
     where CreateViewModel : class, IBaseCrudViewModel, new()
     where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentTeamService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public IncidentTeamService(
            ApplicationDbContext db,
            ILogger<IncidentTeamService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<IncidentTeamModifyViewModel>> GetAllIncidentTeams()
        {
            var list = new List<IncidentTeamModifyViewModel>();
            try
            {
                var teams = await _db.IncidentTeams
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();

                foreach (var t in teams)
                {
                    var vm = new IncidentTeamModifyViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Department = t.Department,
                        Contact = t.Contact,
                        Specializations = t.Specializations,
                        SpecializationList = string.IsNullOrWhiteSpace(t.Specializations)
                            ? new List<string>()
                            : t.Specializations
                                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList()
                    };

                    list.Add(vm);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllIncidentTeams.");
                return new List<IncidentTeamModifyViewModel>();
            }

            return list;
        }

        public async Task<long> SaveIncidentTeam(IncidentTeamModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var userId = 0L; // replace with actual user id from auth context if available

                var team = new IncidentTeam
                {
                    Name = viewModel.Name,
                    Department = viewModel.Department,
                    Contact = viewModel.Contact,
                    Specializations = NormalizeCsvFromList(viewModel.SpecializationList) ?? NormalizeCsv(viewModel.Specializations),
                    IsDeleted = false,
                    ActiveStatus = ActiveStatus.Active,
                    CreatedOn = now,
                    CreatedBy = userId,
                    UpdatedOn = now,
                    UpdatedBy = userId
                };

                await _db.IncidentTeams.AddAsync(team);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return team.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncidentTeam.");
                return 0;
            }
        }

        public async Task<long> UpdateIncidentTeam(IncidentTeamModifyViewModel viewModel)
        {
            try
            {
                var team = await _db.IncidentTeams
                    .FirstOrDefaultAsync(t => t.Id == viewModel.Id);

                if (team == null)
                {
                    // if not found, create new
                    return await SaveIncidentTeam(viewModel);
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                team.Name = viewModel.Name;
                team.Department = viewModel.Department;
                team.Contact = viewModel.Contact;
                // <-- use list-first approach here as well (fallback to CSV)
                team.Specializations = NormalizeCsvFromList(viewModel.SpecializationList) ?? NormalizeCsv(viewModel.Specializations);
                team.UpdatedOn = DateTime.UtcNow;
                // set UpdatedBy if available

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

                return team.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateIncidentTeam.");
                return 0;
            }
        }

        public async Task<IncidentTeamModifyViewModel> GetIncidentTeamById(long id)
        {
            try
            {
                var team = await _db.IncidentTeams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (team == null)
                    return new IncidentTeamModifyViewModel();

                return new IncidentTeamModifyViewModel
                {
                    Id = team.Id,
                    Name = team.Name,
                    Department = team.Department,
                    Contact = team.Contact,
                    Specializations = team.Specializations,
                    SpecializationList = string.IsNullOrWhiteSpace(team.Specializations)
                        ? new List<string>()
                        : team.Specializations
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentTeamById.");
                return new IncidentTeamModifyViewModel();
            }
        }

        public async Task<long> DeleteIncidentTeam(long id)
        {
            try
            {
                var team = await _db.IncidentTeams
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (team == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();

                team.IsDeleted = true;
                team.UpdatedOn = DateTime.UtcNow;
                // set UpdatedBy if available

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

                return team.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteIncidentTeam.");
                return 0;
            }
        }

        /// <summary>
        /// Normalize CSV string: split by comma, trim entries, remove empties and duplicates, then rejoin.
        /// Returns null if there are no valid items.
        /// </summary>
        private static string? NormalizeCsv(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return null;

            var items = csv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return items.Count == 0 ? null : string.Join(",", items);
        }

        /// <summary>
        /// Normalize from list: trim, remove empties and duplicates, then rejoin.
        /// Returns null if there are no valid items.
        /// </summary>
        private static string? NormalizeCsvFromList(List<string>? list)
        {
            if (list == null || list.Count == 0) return null;
            var items = list
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return items.Count == 0 ? null : string.Join(",", items);
        }
    }
}
