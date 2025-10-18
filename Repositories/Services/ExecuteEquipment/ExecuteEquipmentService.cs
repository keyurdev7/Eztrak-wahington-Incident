using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Enums;

namespace Repositories.Common
{
    public class ExecuteEquipmentService :
        IExecuteEquipmentService
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ExecuteEquipmentService> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public ExecuteEquipmentService(
            ApplicationDbContext db,
            ILogger<ExecuteEquipmentService> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            )

        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }




        public async Task<List<EquipmentTransactionIssueViewModel>> GetGroupedOrderTransactionsByEquipments(string poNo)
        {
            try
            {
                var itemsQueryable = (from et in _db.EquipmentTransactions.Where(x => x.PoNo == poNo)
                                      join e in _db.Equipments on et.EquipmentId equals e.Id
                                      //join c in _db.Conditions on et.ConditionId equals c.Id
                                      join o in _db.Orders on et.EntityId equals o.Id
                                      join l in _db.Locations on et.LocationId equals l.Id
                                      join sp in _db.Suppliers on et.SupplierId equals sp.Id
                                      select new
                                      {
                                          e.ItemNo,
                                          et.PoNo,
                                          et.EquipmentId,
                                          e.HourlyRate,
                                          EquipmentName = e.Description ?? "-",
                                          et.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          et.LocationId,
                                          LocationName = l.Name ?? "-",
                                          et.ItemPrice,
                                          et.Quantity,
                                          et.CreatedOn,
                                          o.OrderNumber,
                                          et.ConditionId,
                                          //ConditionName = c.Name,
                                          et.PurchaseDate,
                                          OrderId = o.Id,
                                          EntityDetailId = et.EntityDetailId

                                      }).GroupBy(x => new { x.EquipmentId, x.PoNo, x.SupplierId, x.LocationId, x.ItemPrice, x.HourlyRate, x.PurchaseDate, x.OrderId, x.EntityDetailId })
                                      .Select(x => new EquipmentTransactionIssueViewModel()
                                      {
                                          ItemNo = x.Max(x => x.ItemNo),
                                          PONo = x.Key.PoNo,
                                          Equipment = new EquipmentDetailViewModel()
                                          {
                                              Id = x.Key.EquipmentId,
                                              Description = x.Max(y => y.EquipmentName)
                                          },
                                          Supplier = new SupplierBriefViewModel()
                                          {
                                              Id = x.Key.SupplierId,
                                              Name = x.Max(y => y.SupplierName)
                                          },
                                          PurchaseDate = x.Key.PurchaseDate,
                                          Location = new LocationBriefViewModel()
                                          {
                                              Id = x.Key.LocationId,
                                              Name = x.Max(y => y.LocationName)
                                          },
                                          HourlyRate = x.Key.HourlyRate,
                                          EntityId = x.Key.OrderId,
                                          EntityName = x.Max(x => x.OrderNumber),
                                          EntityDetailId = x.Key.EntityDetailId,
                                          ItemPrice = x.Key.ItemPrice,//x.Average(x => x.ItemPrice)
                                          Quantity = x.Sum(x => x.Quantity),
                                          CreatedOn = x.Max(x => x.CreatedOn),
                                      }).Where(x => x.Quantity < 0).OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionIssueViewModel>();
            }

        }

        public async Task<bool> ReturnEquipments(ReturnEquipmentViewModel viewModel)
        {
            var dbTransactionObj = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var transaction in viewModel.Transactions)
                {
                    var workOrder = await _db.Orders.Include(x => x.WorkOrder).Where(x => x.Id == transaction.EntityId).Select(x => x.WorkOrder).FirstOrDefaultAsync();
                    if (workOrder != null)
                    {
                        var equipmentCost = (transaction.ReturnedQuantity * transaction.Hours * transaction.HourlyRate);
                        workOrder.EquipmentCost += equipmentCost;
                        workOrder.ActualCost += equipmentCost;
                    }
                    var dbTransaction = GetReturnEquipmentTransactionsModel(transaction);
                    _db.EquipmentTransactions.Add(dbTransaction);
                    await _db.SaveChangesAsync();
                }
                await dbTransactionObj.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await dbTransactionObj.RollbackAsync();
                return false;
            }
        }



        private EquipmentTransaction GetReturnEquipmentTransactionsModel(ReturnEquipmentListViewModel item)
        {
            var returnedTransaction = _mapper.Map<EquipmentTransaction>(item);
            returnedTransaction.Quantity = item.ReturnedQuantity;
            returnedTransaction.Hours = item.Hours;
            returnedTransaction.TransactionType = Enums.EquipmentTransactionTypeCatalog.Return;
            return returnedTransaction;

        }


    }
}
