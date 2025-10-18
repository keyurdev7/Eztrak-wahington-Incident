using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Models;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Shared;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Centangle.Common.ResponseHelpers;
using Helpers.File;
using Helpers.Extensions;

namespace Repositories.Common
{
    public class InventoryService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Inventory, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IInventoryService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<InventoryService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IFileHelper _fileHelper;

        public InventoryService(ApplicationDbContext db, ILogger<InventoryService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext, IFileHelper fileHelper) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _fileHelper = fileHelper;
        }

        public override async Task<Expression<Func<Inventory, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as InventorySearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Manufacturer.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.Category.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.UOM.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.MUTCD.Code.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (searchFilters.UOM.Id == null || x.UOM.Id == searchFilters.UOM.Id)
                        &&
                        (searchFilters.Category.Id == null || x.Category.Id == searchFilters.Category.Id)
                        &&
                        (searchFilters.Manufacturer.Id == null || x.Manufacturer.Id == searchFilters.Manufacturer.Id)
                        &&
                        (searchFilters.MUTCD.Id == null || x.MUTCD.Id == searchFilters.MUTCD.Id)
                        ;
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var dbModel = await _db.Inventories
                                        .Include(x => x.Category)
                                        .Include(x => x.UOM)
                                        .Include(x => x.Manufacturer)
                                        .Include(x => x.MUTCD)
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var result = _mapper.Map<DetailViewModel>(dbModel);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Inventory).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Inventory).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetTransactions(long id)
        {
            try
            {
                var shipments = await _db.Transactions
                                        .Include(x => x.Source)
                                        .Include(x => x.Supplier)
                                        .Include(x => x.Location)
                                        .Where(x => x.InventoryId == id)
                                        .ToListAsync();
                if (shipments != null)
                {
                    var result = _mapper.Map<List<ShipmentDetailViewModel>>(shipments);
                    var response = new RepositoryResponseWithModel<List<ShipmentDetailViewModel>> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Transaction).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Transaction).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var searchModel = search as InventorySearchViewModel;
                if (typeof(M) == typeof(InventoryBriefViewModel))
                {
                    return await base.GetAll<M>(search);
                }
                else
                {
                    var query = await GetAllQuerable(search);
                    var dbQuery = query.GroupBy(x => x.Id).Select(g => new InventoryDetailViewModel()
                    {
                        Id = g.Key,
                        ImageUrl = g.Max(x => x.ImageUrl),
                        ItemNo = g.Max(x => x.ItemNo),
                        SystemGeneratedId = g.Max(x => x.SystemGeneratedId),
                        Description = g.Max(x => x.Description),
                        MinimumQuantity = g.Max(x => x.MinimumQuantity),
                        ItemPrice = g.Max(x => x.ItemPrice),
                        Quantity = g.Sum(x => x.Quantity),

                        TotalValue = (float)g.Sum(x => x.TotalPrice),
                        Category = new CategoryBriefViewModel
                        {
                            Id = g.Max(x => x.CategoryId),
                            Name = g.Max(x => x.CategoryName)
                        },
                        Manufacturer = new ManufacturerBriefViewModel
                        {
                            Id = g.Max(x => x.ManufacturerId),
                            Name = g.Max(x => x.ManufacturerName)
                        },
                        UOM = new UOMBriefViewModel
                        {
                            Id = g.Max(x => x.UOMId),
                            Name = g.Max(x => x.UOMName)
                        },
                        MUTCD = new MUTCDBriefViewModel
                        {
                            Id = g.Max(x => x.MUTCId),
                            Code = g.Max(x => x.MUTCCode),
                            Description = g.Max(x => x.MUTCDescription),
                            ImageUrl = g.Max(x => x.MUTCImageUrl)
                        }
                    })
                        //.Where(x => (searchModel.ShowZeroQuantityItems || x.Quantity > 0))
                        .AsQueryable();
                    var paginatedModel = await GetPaginatedResult<M, InventoryDetailViewModel>(searchModel, dbQuery);
                    var responseModel = paginatedModel as RepositoryResponseWithModel<PaginatedResultModel<InventoryDetailViewModel>>;
                    var notes = await _db.InventoryNotes.GroupBy(x => x.InventoryId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count > 0).ToListAsync();
                    foreach (var item in responseModel.ReturnModel.Items)
                    {
                        item.HasNotes = notes.Any(x => x.Id == item.Id);
                    }
                    return paginatedModel;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(InventoryDetailViewModel).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }

        }

        public async Task<double> GetTotalInventoryPrice(IBaseSearchModel search)
        {
            try
            {
                var query = await GetAllQuerable(search);
                return await query.SumAsync(g => g.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTotalInventoryPrice() method for {typeof(Inventory).FullName} threw an exception.");
                return 0;
            }

        }

        public override IQueryable<Inventory> GetPaginationDbSet()
        {
            return _db.Inventories
                .Include(x => x.Category)
                .Include(x => x.UOM)
                .Include(x => x.Manufacturer)
                .Include(x => x.MUTCD)
                .AsQueryable();
        }

        public async Task<List<InventoryNotesViewModel>> GetNotesByInventoryId(int id)
        {
            try
            {
                var notes = await (from n in _db.InventoryNotes.Include(x => x.Inventory)
                                   join u in _db.Users on n.CreatedBy equals u.Id
                                   where (n.InventoryId == id)
                                   select new InventoryNotesViewModel
                                   {
                                       Description = n.Description,
                                       FileUrl = n.FileUrl,
                                       CreatedOn = n.CreatedOn,
                                       CreatedBy = u.FirstName + " " + u.LastName,
                                   }).ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<List<InventoryCostViewModel>> GetInventoryAverageCost(List<long> ids)
        {
            try
            {
                var inventoriesCosts = await (from i in _db.Inventories
                                              join t in _db.Transactions.Where(x => x.TransactionType == Enums.TransactionTypeCatalog.Shipment)
                                                on i.Id equals t.InventoryId
                                              where (ids.Contains(i.Id))
                                              select new
                                              {
                                                  InventoryId = i.Id,
                                                  ItemPrice = t.ItemPrice,
                                              }).GroupBy(x => x.InventoryId)
                                   .Select(x => new InventoryCostViewModel
                                   {
                                       InventoryId = x.Key,
                                       AverageCost = x.Average(y => y.ItemPrice)
                                   }).ToListAsync();
                return inventoriesCosts;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<bool> SaveNotes(InventoryNotesViewModel model)
        {
            try
            {
                model.FileUrl = _fileHelper.Save(model);
                var mappedNotes = _mapper.Map<InventoryNotes>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateShipments(ShipmentGridViewModel model)
        {
            try
            {
                var shipments = _mapper.Map<List<Transaction>>(model.Shipments);
                await _db.AddRangeAsync(shipments);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<IQueryable<InventoryViewModel>> GetAllQuerable(IBaseSearchModel search)
        {
            var filters = await SetQueryFilter(search);
            return (from i in _db.Inventories.Where(filters)
                    join t in _db.Transactions on i.Id equals t.InventoryId into tl
                    from t in tl.DefaultIfEmpty()
                    join c in _db.Categories on i.CategoryId equals c.Id
                    join m in _db.Manufacturers on i.ManufacturerId equals m.Id
                    join uom in _db.UOMs on i.UOMId equals uom.Id
                    join mutc in _db.MUTCDs on i.MUTCDId equals mutc.Id into mutcl
                    from mutc in mutcl.DefaultIfEmpty()
                    select new InventoryViewModel
                    {
                        Id = i.Id,
                        ItemNo = i.ItemNo,
                        SystemGeneratedId = i.SystemGeneratedId,
                        Description = i.Description,
                        ImageUrl = i.ImageUrl,
                        MinimumQuantity = i.MinimumQuantity,
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        ManufacturerId = m.Id,
                        ManufacturerName = m.Name,
                        UOMId = uom.Id,
                        UOMName = uom.Name,
                        Quantity = t == null ? 0 : t.Quantity,
                        ItemPrice = t == null ? 0 : t.ItemPrice,
                        TotalPrice = t == null ? 0 : (t.Quantity * t.ItemPrice),
                        MUTCId = (mutc == null ? 0 : mutc.Id),
                        MUTCCode = (mutc == null ? "" : mutc.Code),
                        MUTCImageUrl = (mutc == null ? "" : mutc.ImageUrl),
                        MUTCDescription = (mutc == null ? "" : mutc.Description),
                    });
        }

        public async Task<List<TransactionHistoryViewModel>> GetInventoryIssueHistory(int inventoryId, string lotNo, int locationId, int sourceId)
        {
            try
            {
                var history = await (from t in _db.Transactions
                                     join loc in _db.Locations on t.LocationId equals loc.Id
                                     join u in _db.Users on t.CreatedBy equals u.Id
                                     where t.InventoryId == inventoryId
                                     && t.IsDeleted == false
                                     && (locationId == 0 || t.LocationId == locationId)
                                     && (string.IsNullOrEmpty(lotNo) || t.LotNO == lotNo)
                                     && (sourceId == 0 || t.SourceId == sourceId)
                                     select new TransactionHistoryViewModel
                                     {
                                         Id = t.Id,
                                         InventoryId = t.InventoryId,
                                         TransactionType = t.TransactionType,
                                         LotNO = t.LotNO,
                                         Quantity = t.Quantity,
                                         CreatedBy = u.FirstName + " " + u.LastName,
                                         Location = new LocationBriefViewModel
                                         {
                                             Id = loc.Id,
                                             Name = loc.Name
                                         },
                                         CreatedOn = t.CreatedOn,
                                     })
                              .OrderByDescending(x => x.CreatedOn)
                              .ToListAsync();
                return history;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as InventoryModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            var totalInventoryCount = await _db.Inventories.IgnoreQueryFilters().CountAsync();
            viewModel.SystemGeneratedId = "INV-" + (totalInventoryCount + 1).ToString("D4");
            return await base.Create(model);
        }

        public override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var viewModel = model as InventoryModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            return base.Update(model);
        }

        public async Task<bool> IsItemNoUnique(long id, string itemNo)
        {
            return (await _db.Inventories.Where(x => x.ItemNo == itemNo && x.Id != id && x.IsDeleted == false).CountAsync()) < 1;
        }
    }
}
