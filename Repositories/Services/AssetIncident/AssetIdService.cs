using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using Enums; // for ActiveStatus enum

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
    public class AssetIdService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<AssetIncident, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IAssetIdService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AssetIdService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public AssetIdService(
            ApplicationDbContext db,
            ILogger<AssetIdService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<AssetIdModifyViewModel>> GetAllAssetIds()
        {
            var list = new List<AssetIdModifyViewModel>();
            try
            {
                var assets = await _db.AssetIncidents
                    .Where(a => !a.IsDeleted)
                    .ToListAsync();

                foreach (var a in assets)
                {
                    list.Add(new AssetIdModifyViewModel
                    {
                        Id = a.Id,
                        Name = a.Name
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAssetIds.");
                return new List<AssetIdModifyViewModel>()!;
            }
            return list;
        }

        public async Task<long> SaveAssetId(AssetIdModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var userId = 0L; // set from your auth context if available

                var asset = new AssetIncident
                {
                    Name = viewModel.Name,
                    IsDeleted = false,
                    // Use enum safely; if your enum name differs, cast: (ActiveStatus)1
                    ActiveStatus = ActiveStatus.Active,
                    CreatedOn = now,
                    CreatedBy = userId,
                    UpdatedOn = now,
                    UpdatedBy = userId
                };

                await _db.AssetIncidents.AddAsync(asset);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return asset.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveAssetId.");
                return 0;
            }
        }

        public async Task<long> UpdateAssetId(AssetIdModifyViewModel viewModel)
        {
            try
            {
                var asset = await _db.AssetIncidents
                    .FirstOrDefaultAsync(a => a.Id == viewModel.Id);

                if (asset == null)
                {
                    // create new if not found
                    return await SaveAssetId(viewModel);
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                asset.Name = viewModel.Name;
                asset.UpdatedOn = DateTime.UtcNow;
                // keep previous UpdatedBy or set from current user if you have it

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

                return asset.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAssetId.");
                return 0;
            }
        }

        public async Task<AssetIdModifyViewModel> GetAssetIdById(long id)
        {
            try
            {
                var asset = await _db.AssetIncidents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (asset == null)
                    return new AssetIdModifyViewModel();

                return new AssetIdModifyViewModel
                {
                    Id = asset.Id,
                    Name = asset.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAssetIdById.");
                return new AssetIdModifyViewModel();
            }
        }

        public async Task<long> DeleteAssetId(long id)
        {
            try
            {
                var asset = await _db.AssetIncidents
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (asset == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();

                asset.IsDeleted = true;
                asset.UpdatedOn = DateTime.UtcNow;

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

                return asset.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAssetId.");
                return 0;
            }
        }
    }
}