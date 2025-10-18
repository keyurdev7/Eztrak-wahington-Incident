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
using Enums;

namespace Repositories.Common
{
    public class TransactionService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Transaction, CreateViewModel, UpdateViewModel, DetailViewModel>,
        ITransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public TransactionService(
            ApplicationDbContext db,
            ILogger<TransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            )
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public override async Task<Expression<Func<Transaction, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as TransactionSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Source.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.Supplier.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.Location.Name.ToLower().Contains(searchFilters.Search.value.ToLower())

                            )
                        )
                        &&
                        (searchFilters.Source.Id == null || x.Source.Id == searchFilters.Source.Id)
                        &&
                        (searchFilters.Supplier.Id == null || x.Supplier.Id == searchFilters.Supplier.Id)
                          &&
                        (searchFilters.Location.Id == null || x.Location.Id == searchFilters.Location.Id)
                        ;
        }

        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "Inventory", "Location", "Source", "Supplier" };
        }

        public async Task<List<TransactionDetailViewModel>> GetGroupedTransactionsByItems(List<long> inventoryId)
        {
            try
            {
                var itemsQueryable = (from t in _db.Transactions.Where(x => inventoryId.Contains(x.InventoryId))
                                      join i in _db.Inventories on t.InventoryId equals i.Id
                                      join s in _db.Sources on t.SourceId equals s.Id into sl
                                      from s in sl.DefaultIfEmpty()
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      select new
                                      {
                                          t.LotNO,
                                          t.InventoryId,
                                          InventoryName = i.Description ?? "-",
                                          t.SourceId,
                                          SourceName = (s == null ? "-" : s.Name ?? "-"),
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,
                                          t.CreatedOn

                                      }).GroupBy(x => new { x.InventoryId, x.LotNO, x.SourceId, x.SupplierId, x.LocationId, x.ItemPrice })
                                      .Select(x => new TransactionDetailViewModel()
                                      {
                                          LotNO = x.Key.LotNO,
                                          Inventory = new InventoryDetailViewModel()
                                          {
                                              Id = x.Key.InventoryId,
                                              Description = x.Max(y => y.InventoryName)
                                          },
                                          Source = new SourceBriefViewModel()
                                          {
                                              Id = x.Key.SourceId,
                                              Name = x.Max(y => y.SourceName)
                                          },
                                          Supplier = new SupplierBriefViewModel()
                                          {
                                              Id = x.Key.SupplierId,
                                              Name = x.Max(y => y.SupplierName)
                                          },
                                          Location = new LocationBriefViewModel()
                                          {
                                              Id = x.Key.LocationId,
                                              Name = x.Max(y => y.LocationName)
                                          },
                                          ItemPrice = x.Key.ItemPrice,//x.Average(x => x.ItemPrice)
                                          Quantity = x.Sum(x => x.Quantity),
                                          CreatedOn = x.Max(x => x.CreatedOn),
                                      }).Where(x => x.Quantity > 0).OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<TransactionDetailViewModel>();
            }
        }

        public async Task<List<TransactionDetailViewModel>> GetGroupedTransactionsByItemsForOrder(List<long> inventoryId)
        {
            try
            {
                var itemsQueryable = (from t in _db.Transactions.Where(x => inventoryId.Contains(x.InventoryId))
                                      join i in _db.Inventories.Include(x => x.UOM) on t.InventoryId equals i.Id
                                      join s in _db.Sources on t.SourceId equals s.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      //where inventoryId.Contains(t.InventoryId)
                                      //group t by new { t.InventoryId, t.LotNO, s, t.SupplierId, t.LocationId } into g
                                      select new
                                      {
                                          t.LotNO,
                                          t.InventoryId,
                                          InventoryName = i.Description ?? "-",
                                          InventoryUOMId = i.UOM.Id,
                                          InventoryUOMName = i.UOM.Name,
                                          InventoryItemNo = i.ItemNo,
                                          t.SourceId,
                                          SourceName = s.Name ?? "-",
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,

                                      }).GroupBy(x => new { x.InventoryId })
                                      .Select(x => new TransactionDetailViewModel()
                                      {
                                          LotNO = x.Max(y => y.LotNO) ?? "-",
                                          Inventory = new InventoryDetailViewModel()
                                          {
                                              Id = x.Key.InventoryId,
                                              Description = x.Max(y => y.InventoryName),
                                              ItemNo = x.Max(y => y.InventoryItemNo),
                                              UOM = new UOMBriefViewModel
                                              {
                                                  Id = x.Max(y => y.InventoryUOMId),
                                                  Name = x.Max(y => y.InventoryUOMName)
                                              }
                                          },
                                          Source = new SourceBriefViewModel()
                                          {
                                              Id = x.Max(y => y.SourceId),
                                              Name = x.Max(y => y.SourceName)
                                          },
                                          Supplier = new SupplierBriefViewModel()
                                          {
                                              Id = x.Max(y => y.SupplierId),
                                              Name = x.Max(y => y.SupplierName)
                                          },
                                          Location = new LocationBriefViewModel()
                                          {
                                              Id = x.Max(y => y.LocationId),
                                              Name = x.Max(y => y.LocationName)
                                          },
                                          ItemPrice = x.Average(x => x.ItemPrice),
                                          Quantity = x.Sum(x => x.Quantity)
                                      }).Where(x => x.Quantity > 0).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<TransactionDetailViewModel>();
            }

        }

        public async Task<List<TransactionDetailViewModel>> GetWorkOrderTransactions(string workOrderId)
        {
            try
            {
                var itemsQueryable = (from t in _db.Transactions.Where(x => x.TransactionType == TransactionTypeCatalog.Order || x.TransactionType == TransactionTypeCatalog.Return)
                                      join i in _db.Inventories on t.InventoryId equals i.Id
                                      join uom in _db.UOMs on i.UOMId equals uom.Id into uoml
                                      from uom in uoml.DefaultIfEmpty()
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      join o in _db.Orders on t.EntityId equals o.Id
                                      join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                      where wo.SystemGeneratedId.Equals(workOrderId)
                                      select new TransactionDetailViewModel()
                                      {
                                          Inventory = new InventoryDetailViewModel()
                                          {
                                              Id = i.Id,
                                              ItemNo = i.ItemNo,
                                              UOM = new UOMBriefViewModel
                                              {
                                                  Id = uom == null ? 0 : uom.Id,
                                                  Name = uom == null ? "" : uom.Name
                                              },
                                              Description = i.Description
                                          },
                                          Supplier = new SupplierBriefViewModel()
                                          {
                                              Id = sp.Id,
                                              Name = sp.Name
                                          },
                                          Location = new LocationBriefViewModel()
                                          {
                                              Id = l.Id,
                                              Name = l.Name
                                          },
                                          ItemPrice = t.ItemPrice,
                                          LotNO = t.LotNO,
                                          Quantity = t.Quantity,
                                          CreatedOn = t.CreatedOn,
                                      }).OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<TransactionDetailViewModel>();
            }
        }
    }
}
