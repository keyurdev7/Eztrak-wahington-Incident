using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class EquipmentFieldsService<CreateViewModel, UpdateViewModel, DetailViewModel>
    : BaseService<EquipmentFields, CreateViewModel, UpdateViewModel, DetailViewModel>,
      IEquipmentFieldsService<CreateViewModel, UpdateViewModel, DetailViewModel>
    where DetailViewModel : class, IBaseCrudViewModel, new()
    where CreateViewModel : class, IBaseCrudViewModel, new()
    where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EquipmentFieldsService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        public EquipmentFieldsService(
            ApplicationDbContext db,
            ILogger<EquipmentFieldsService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<EquipmentFieldsModifyViewModel>> GetAllEquipmentFields()
        {
            List<EquipmentFieldsModifyViewModel> equipmentfields = new();
            try
            {

                var equipmentfield = await _db.EquipmentFields.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var EquipmentField in equipmentfield)
                {
                    equipmentfields.Add(new EquipmentFieldsModifyViewModel()
                    {
                        Id = EquipmentField.Id,
                        EquipmentFieldsID = EquipmentField.EquipmentFieldsID,
                        Name = EquipmentField.Name,
                        Category = EquipmentField.Category,
                        HourlyRate = EquipmentField.HourlyRate
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Equipment Fields.");
                return new List<EquipmentFieldsModifyViewModel>()!;
            }
            return equipmentfields;
        }
        public async Task<long> SaveEquipmentFields(EquipmentFieldsModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var totalEquipmentFieldsCount = await _db.EquipmentFields.IgnoreQueryFilters().CountAsync();
                var EquipmentFieldsID = $"EQ-{(totalEquipmentFieldsCount + 1):D4}";
                // Map ViewModel → Entity
                var equipmentfields = new EquipmentFields
                {
                    EquipmentFieldsID = EquipmentFieldsID,
                    Name = viewModel.Name,
                    Category = viewModel.Category,
                    HourlyRate = viewModel.HourlyRate
                };

                // Save
                await _db.EquipmentFields.AddAsync(equipmentfields);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return equipmentfields.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error Save Equipment Field.");
                return 0;
            }
        }
        public async Task<long> UpdateEquipmentFields(EquipmentFieldsModifyViewModel viewModel)
        {
            try
            {

                var equipmentfields = await _db.EquipmentFields.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (equipmentfields == null)
                {
                    await SaveEquipmentFields(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                equipmentfields.Name = viewModel.Name;
                equipmentfields.Category = viewModel.Category;
                equipmentfields.HourlyRate = viewModel.HourlyRate;

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

                return equipmentfields.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Save Equipment Field.");
                return 0;
            }
        }
        public async Task<EquipmentFieldsModifyViewModel> GetEquipmentFieldsById(long Id)
        {
            var equipmentfieldsView = new EquipmentFieldsModifyViewModel();

            try
            {
                var equipmentfields = await _db.EquipmentFields.FirstOrDefaultAsync(p => p.Id == Id);

                if (equipmentfields == null)
                {
                    return new EquipmentFieldsModifyViewModel();
                }
                equipmentfieldsView.Id = equipmentfields.Id;
                equipmentfieldsView.EquipmentFieldsID = equipmentfields.EquipmentFieldsID;
                equipmentfieldsView.Name = equipmentfields.Name;
                equipmentfieldsView.Category = equipmentfields.Category;
                equipmentfieldsView.HourlyRate = equipmentfields.HourlyRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById.");
                return new EquipmentFieldsModifyViewModel();
            }

            return equipmentfieldsView;
        }
        public async Task<long> DeleteEquipmentFields(long Id)
        {
            try
            {

                var equipmentfields = await _db.EquipmentFields.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (equipmentfields == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                equipmentfields.IsDeleted = true;

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

                return equipmentfields.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete Equipment Field.");
                return 0;
            }
        }
    }
}
