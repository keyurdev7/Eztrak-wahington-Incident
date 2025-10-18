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
using System.Threading.Tasks;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class IncidentShiftService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<IncidentShift, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IIncidentShiftService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentShiftService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public IncidentShiftService(
            ApplicationDbContext db,
            ILogger<IncidentShiftService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
        ) : base(db, logger, mapper, response)
        {
            // defensive: avoid null reference if actionContext not set
            _modelState = actionContext?.ActionContext?.ModelState ?? new ModelStateDictionary();
            _db = db;
            _logger = logger;
        }

        public async Task<List<IncidentShiftModifyViewModel>> GetAllIncidentShifts()
        {
            var list = new List<IncidentShiftModifyViewModel>();
            try
            {
                var items = await _db.IncidentShifts.Where(p => !p.IsDeleted).ToListAsync();
                foreach (var i in items)
                {
                    list.Add(new IncidentShiftModifyViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllIncidentShifts.");
                return new List<IncidentShiftModifyViewModel>();
            }
            return list;
        }

        public async Task<long> SaveIncidentShift(IncidentShiftModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = new IncidentShift
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description ?? string.Empty
                };

                await _db.IncidentShifts.AddAsync(entity);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return entity.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncidentShift.");
                return 0;
            }
        }

        public async Task<long> UpdateIncidentShift(IncidentShiftModifyViewModel viewModel)
        {
            try
            {
                var entity = await _db.IncidentShifts.FirstOrDefaultAsync(p => p.Id == viewModel.Id);
                if (entity == null)
                    return await SaveIncidentShift(viewModel);

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
                _logger.LogError(ex, "Error UpdateIncidentShift.");
                return 0;
            }
        }

        public async Task<IncidentShiftModifyViewModel> GetIncidentShiftById(long Id)
        {
            var vm = new IncidentShiftModifyViewModel();
            try
            {
                var entity = await _db.IncidentShifts.FirstOrDefaultAsync(p => p.Id == Id);
                if (entity == null) return new IncidentShiftModifyViewModel();

                vm.Id = entity.Id;
                vm.Name = entity.Name;
                vm.Description = entity.Description;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentShiftById.");
                return new IncidentShiftModifyViewModel();
            }
            return vm;
        }

        public async Task<long> DeleteIncidentShift(long Id)
        {
            try
            {
                var entity = await _db.IncidentShifts.FirstOrDefaultAsync(p => p.Id == Id);
                if (entity == null) return 0;

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
                _logger.LogError(ex, "Error DeleteIncidentShift.");
                return 0;
            }
        }
    }
}
