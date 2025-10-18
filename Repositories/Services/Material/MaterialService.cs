// Repositories/Common/MaterialService.cs
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Enums;
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
    public class MaterialService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<Material, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IMaterialService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<MaterialService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public MaterialService(
            ApplicationDbContext db,
            ILogger<MaterialService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<MaterialModifyViewModel>> GetAllMaterials()
        {
            var list = new List<MaterialModifyViewModel>();
            try
            {
                var items = await _db.Materials
                    .Where(m => !m.IsDeleted)
                    .ToListAsync();

                foreach (var m in items)
                {
                    list.Add(new MaterialModifyViewModel
                    {
                        Id = m.Id,
                       //MaterialID = m.MaterialID,
                        Name = m.Name,
                        Category = m.Category,
                        Unit = m.Unit,
                        UnitCost = m.UnitCost ?? 0f
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllMaterials.");
                return new List<MaterialModifyViewModel>();
            }

            return list;
        }

        public async Task<long> SaveMaterial(MaterialModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var userId = 0L;
                var materialId = string.IsNullOrWhiteSpace(viewModel.MaterialID)
            ? "MAT01"
            : viewModel.MaterialID;

                var material = new Material
                {
                    //MaterialID = materialId,
                    Name = viewModel.Name,
                    Category = viewModel.Category,
                    Unit = viewModel.Unit,
                    UnitCost = viewModel.UnitCost,
                    IsDeleted = false,
                    ActiveStatus = ActiveStatus.Active,
                    CreatedOn = now,
                    CreatedBy = userId,
                    UpdatedOn = now,
                    UpdatedBy = userId
                };

                await _db.Materials.AddAsync(material);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return material.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveMaterial.");
                return 0;
            }
        }

        public async Task<long> UpdateMaterial(MaterialModifyViewModel viewModel)
        {
            try
            {
                var material = await _db.Materials
                    .FirstOrDefaultAsync(m => m.Id == viewModel.Id);

                if (material == null)
                {
                    return await SaveMaterial(viewModel);
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                //material.MaterialID = viewModel.MaterialID;
                material.Name = viewModel.Name;
                material.Category = viewModel.Category;
                material.Unit = viewModel.Unit;
                material.UnitCost = viewModel.UnitCost;
                material.UpdatedOn = DateTime.UtcNow;

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

                return material.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateMaterial.");
                return 0;
            }
        }

        public async Task<MaterialModifyViewModel> GetMaterialById(long id)
        {
            try
            {
                var m = await _db.Materials.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (m == null)
                    return new MaterialModifyViewModel();

                return new MaterialModifyViewModel
                {
                    Id = m.Id,
                    MaterialID = m.MaterialID,
                    Name = m.Name,
                    Category = m.Category,
                    Unit = m.Unit,
                    UnitCost = m.UnitCost ?? 0f
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetMaterialById.");
                return new MaterialModifyViewModel();
            }
        }

        public async Task<long> DeleteMaterial(long id)
        {
            try
            {
                var material = await _db.Materials.FirstOrDefaultAsync(m => m.Id == id);

                if (material == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();

                material.IsDeleted = true;
                material.UpdatedOn = DateTime.UtcNow;

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

                return material.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteMaterial.");
                return 0;
            }
        }
    }
}
