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
    public class EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<EquipmentTransaction, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IEquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public EquipmentTransactionService(
            ApplicationDbContext db,
            ILogger<EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
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

        public override async Task<Expression<Func<EquipmentTransaction, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as EquipmentTransactionSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Supplier.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.Location.Name.ToLower().Contains(searchFilters.Search.value.ToLower())

                            )
                        )
                        &&
                        (searchFilters.Supplier.Id == null || x.Supplier.Id == searchFilters.Supplier.Id)
                          &&
                        (searchFilters.Location.Id == null || x.Location.Id == searchFilters.Location.Id)

                        ;
        }

        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "Equipment", "Location", "Source", "Supplier" };
        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItems(List<long> EquipmentId)
        {
            try
            {
                var itemsQueryable = (from t in _db.EquipmentTransactions.Where(x => EquipmentId.Contains(x.EquipmentId))
                                      join i in _db.Equipments on t.EquipmentId equals i.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      select new
                                      {
                                          t.PoNo,
                                          t.EquipmentId,
                                          EquipmentName = i.Description ?? "-",
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,
                                          t.Equipment.HourlyRate,
                                          t.CreatedOn,
                                          t.PurchaseDate

                                      }).GroupBy(x => new { x.PoNo, x.EquipmentId, x.SupplierId, x.LocationId, x.ItemPrice, x.PurchaseDate })
                                      .Select(x => new EquipmentTransactionDetailViewModel()
                                      {
                                          Equipment = new EquipmentDetailViewModel()
                                          {
                                              Id = x.Key.EquipmentId,
                                              HourlyRate = x.Max(x => x.HourlyRate),
                                              Description = x.Max(y => y.EquipmentName)
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
                                          PONo = x.Key.PoNo,
                                          HourlyRate = x.Max(x => x.HourlyRate),//x.Average(x => x.ItemPrice)
                                          Quantity = x.Sum(x => x.Quantity),
                                          CreatedOn = x.Max(x => x.CreatedOn),
                                          PurchaseDate = x.Key.PurchaseDate
                                      })
                                      //.Where(x => x.Quantity > 0)
                                      .OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }
        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItemsForOrder(List<long> EquipmentId)
        {
            try
            {
                var itemsQueryable = (from t in _db.EquipmentTransactions.Where(x => EquipmentId.Contains(x.EquipmentId))
                                      join i in _db.Equipments.Include(x => x.UOM).Include(x => x.Manufacturer) on t.EquipmentId equals i.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      //where EquipmentId.Contains(t.EquipmentId)
                                      //group t by new { t.EquipmentId, t.LotNO, s, t.SupplierId, t.LocationId } into g
                                      select new
                                      {
                                          t.EquipmentId,
                                          t.PoNo,
                                          EquipmentName = i.Description ?? "-",
                                          EquipmentUOMId = i.UOM.Id,
                                          EquipmentUOMName = i.UOM.Name,
                                          EquipmentItemNo = i.ItemNo,
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,
                                          t.HourlyRate,
                                          ManufacturerId = i.Manufacturer.Id,
                                          ManufacturerName = i.Manufacturer.Name


                                      }).GroupBy(x => new { x.EquipmentId })
                                      .Select(x => new EquipmentTransactionDetailViewModel()
                                      {
                                          Equipment = new EquipmentDetailViewModel()
                                          {
                                              Id = x.Key.EquipmentId,
                                              Description = x.Max(y => y.EquipmentName),
                                              ItemNo = x.Max(y => y.EquipmentItemNo),
                                              UOM = new UOMBriefViewModel
                                              {
                                                  Id = x.Max(y => y.EquipmentUOMId),
                                                  Name = x.Max(y => y.EquipmentUOMName)
                                              },
                                              Manufacturer = new ManufacturerBriefViewModel
                                              {
                                                  Id = x.Max(y => y.ManufacturerId),
                                                  Name = x.Max(y => y.ManufacturerName)
                                              }
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
                                          HourlyRate = x.Average(x => x.HourlyRate),
                                          Quantity = x.Sum(x => x.Quantity)
                                      }).Where(x => x.Quantity > 0).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }

        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetWorkOrderTransactions(string workOrderId)
        {
            try
            {
                var itemsQueryable = (from t in _db.EquipmentTransactions.Where(x => x.TransactionType == EquipmentTransactionTypeCatalog.Order || x.TransactionType == EquipmentTransactionTypeCatalog.Return)
                                      join e in _db.Equipments on t.EquipmentId equals e.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      join o in _db.Orders on t.EntityId equals o.Id
                                      join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                      where wo.SystemGeneratedId.Equals(workOrderId)
                                      select new EquipmentTransactionDetailViewModel()
                                      {
                                          Equipment = new EquipmentDetailViewModel()
                                          {
                                              Id = e.Id,
                                              HourlyRate = e.HourlyRate,
                                              Description = e.Description
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
                                          TransactionType = t.TransactionType,
                                          ItemPrice = t.ItemPrice,//x.Average(x => x.ItemPrice)
                                          PONo = t.PoNo,
                                          Hours = t.Hours,
                                          HourlyRate = e.HourlyRate,//x.Average(x => x.ItemPrice)
                                          Quantity = t.Quantity,
                                          CreatedOn = t.CreatedOn,
                                          PurchaseDate = t.PurchaseDate
                                      }).OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }
        }

    }
}
