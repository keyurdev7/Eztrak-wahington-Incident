using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.InkML;

using Enums;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Shared;


namespace Repositories.Common
{
    public class AssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<AssetType, CreateViewModel, UpdateViewModel, DetailViewModel>, IAssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public AssetTypeService(
            ApplicationDbContext db,
            ILogger<AssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<AssetTypeModifyViewModel>> GetAllAssetTypes()
        {
            var list = new List<AssetTypeModifyViewModel>();
            try
            {
                var types = await _db.AssetTypes
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();

                foreach (var t in types)
                {
                    list.Add(new AssetTypeModifyViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Color = t.Color
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAssetTypes.");
                return new List<AssetTypeModifyViewModel>()!;
            }
            return list;
        }

        public async Task<long> SaveAssetType(AssetTypeModifyViewModel viewModel)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var userId = 0L; // TODO: plug in current user id if available

                var entity = new AssetType
                {
                    Name = viewModel.Name,
                    Color = viewModel.Color,
                    IsDeleted = false,
                    ActiveStatus = ActiveStatus.Active, // or (ActiveStatus)1
                    CreatedOn = now,
                    CreatedBy = userId,
                    UpdatedOn = now,
                    UpdatedBy = userId
                };

                await _db.AssetTypes.AddAsync(entity);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return entity.Id;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error SaveAssetType.");
                return 0;
            }
        }

        public async Task<long> UpdateAssetType(AssetTypeModifyViewModel viewModel)
        {
            try
            {
                var entity = await _db.AssetTypes
                    .FirstOrDefaultAsync(t => t.Id == viewModel.Id && !t.IsDeleted);

                if (entity == null)
                {
                    // Create if not found (same behavior as AssetId service)
                    return await SaveAssetType(viewModel);
                }

                await using var tx = await _db.Database.BeginTransactionAsync();

                entity.Name = viewModel.Name;
                entity.Color = viewModel.Color;
                entity.UpdatedOn = DateTime.UtcNow;
                // entity.UpdatedBy = currentUserId; // set if available

                try
                {
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAssetType.");
                return 0;
            }
        }

        public async Task<AssetTypeModifyViewModel> GetAssetTypeById(long id)
        {
            try
            {
                var entity = await _db.AssetTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (entity == null)
                    return new AssetTypeModifyViewModel();

                return new AssetTypeModifyViewModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Color = entity.Color
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAssetTypeById.");
                return new AssetTypeModifyViewModel();
            }
        }

        public async Task<long> DeleteAssetType(long id)
        {
            try
            {
                var entity = await _db.AssetTypes
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (entity == null)
                    return 0;

                await using var tx = await _db.Database.BeginTransactionAsync();

                entity.IsDeleted = true;
                entity.UpdatedOn = DateTime.UtcNow;
                // entity.UpdatedBy = currentUserId;

                try
                {
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAssetType.");
                return 0;
            }
        }


        //public override async Task<Expression<Func<AssetType, bool>>> SetQueryFilter(IBaseSearchModel filters)
        //{
        //    var searchFilters = filters as AssetTypeSearchViewModel;

        //    return x =>
        //                (
        //                    (
        //                        string.IsNullOrEmpty(searchFilters.Search.value)
        //                        ||
        //                        x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                    )
        //                )
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
        //                ;
        //}


        //public async Task<List<AssetTypeTreeViewModel>> GetHirearchy()
        //{
        //    var assetTree = await _db.AssetTypes.Include(at => at.AssetTypeLevel1)
        //            .ThenInclude(atl1 => atl1.AssetTypeLevel2)
        //        .Select(at => new AssetTypeTreeViewModel
        //        {
        //            Id= at.Id,
        //            Name = at.Name,
        //            AssetTypeLevel1 = at.AssetTypeLevel1.Select(atl1 => new AssetTypeLevel1TreeViewModel
        //            {
        //                Id = atl1.Id,
        //                Name = atl1.Name,
        //                AssetTypeLevel2 = atl1.AssetTypeLevel2.Select(atl2 => new AssetTypeLevel2TreeViewModel
        //                {
        //                    Id = atl2.Id,
        //                    Name = atl2.Name
        //                }).ToList()
        //            }).ToList()
        //        }).ToListAsync();
        //    return assetTree;
        //}

    }
}
