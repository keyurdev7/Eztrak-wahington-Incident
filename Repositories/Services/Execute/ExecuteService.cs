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
    public class ExecuteService :
        IExecuteService
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ExecuteService> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public ExecuteService(
            ApplicationDbContext db,
            ILogger<ExecuteService> logger,
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


        public async Task<List<TransactionIssueViewModel>> GetGroupedTransactionsByItems(string lotNo)
        {
            try
            {
                var itemsQueryable = (from t in _db.Transactions.Where(x => x.LotNO == lotNo)
                                      join i in _db.Inventories on t.InventoryId equals i.Id
                                      join s in _db.Sources on t.SourceId equals s.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      select new
                                      {
                                          t.LotNO,
                                          t.InventoryId,
                                          InventoryName = i.Description ?? "-",
                                          t.SourceId,
                                          SourceName = s.Name ?? "-",
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,
                                          t.CreatedOn

                                      }).GroupBy(x => new { x.InventoryId, x.LotNO, x.SourceId, x.SupplierId, x.LocationId, x.ItemPrice })
                                      .Select(x => new TransactionIssueViewModel()
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
                return new List<TransactionIssueViewModel>();
            }

        }

        public async Task<List<TransactionIssueViewModel>> GetGroupedOrderTransactionsByItems(string lotNo)
        {
            try
            {
                var itemsQueryable = (from t in _db.Transactions.Where(x => x.LotNO == lotNo && (x.TransactionType == TransactionTypeCatalog.Order || x.TransactionType == TransactionTypeCatalog.Return))
                                      join o in _db.Orders on t.EntityId equals o.Id
                                      join i in _db.Inventories on t.InventoryId equals i.Id
                                      join s in _db.Sources on t.SourceId equals s.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      select new
                                      {
                                          t.LotNO,
                                          t.InventoryId,
                                          InventoryName = i.Description ?? "-",
                                          t.SourceId,
                                          SourceName = s.Name ?? "-",
                                          t.SupplierId,
                                          SupplierName = sp.Name ?? "-",
                                          t.LocationId,
                                          LocationName = l.Name ?? "-",
                                          t.ItemPrice,
                                          t.Quantity,
                                          t.CreatedOn,
                                          o.OrderNumber,
                                          OrderId = o.Id,
                                          EntityDetailId = t.EntityDetailId

                                      }).GroupBy(x => new { x.InventoryId, x.LotNO, x.SourceId, x.SupplierId, x.LocationId, x.ItemPrice, x.OrderId, x.EntityDetailId })
                                      .Select(x => new TransactionIssueViewModel()
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
                return new List<TransactionIssueViewModel>();
            }

        }

        public async Task<bool> ReStageItems(ReStageViewModel viewModel)
        {
            try
            {
                foreach (var transaction in viewModel.Transactions)
                {
                    _db.Transactions.AddRange(GetReStageTransactionsModel(transaction));
                }
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveInventoryItems(RemoveInventoryItemsViewModel viewModel)
        {
            var dbTransaction = await _db.Database.BeginTransactionAsync();

            try
            {
                foreach (var transaction in viewModel.Transactions)
                {
                    var removedInventory = new RemovedInventory()
                    {
                        Justification = transaction.Justification,
                    };
                    _db.RemovedInventories.Add(removedInventory);
                    await _db.SaveChangesAsync();
                    _db.Transactions.Add(GetRemoveInventoryTransactionsModel(transaction, removedInventory.Id));
                }
                var isSuccessful = await _db.SaveChangesAsync() > 0;
                await dbTransaction.CommitAsync();
                return isSuccessful;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return false;
            }

        }

        public async Task<bool> ReturnInventoryItems(ReturnInventoryItemsViewModel viewModel)
        {
            try
            {
                foreach (var transaction in viewModel.Transactions)
                {
                    var dbTransaction = GetReturnInventoryTransactionsModel(transaction);
                    _db.Transactions.Add(dbTransaction);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<Transaction> GetReStageTransactionsModel(ReStageListViewModel item)
        {
            var removedTransaction = _mapper.Map<Transaction>(item);
            removedTransaction.Quantity = item.NewQuantity * -1;
            removedTransaction.TransactionType = Enums.TransactionTypeCatalog.ReStage;
            var addedTransaction = _mapper.Map<Transaction>(item);
            addedTransaction.Quantity = item.NewQuantity;
            addedTransaction.LocationId = item.NewLocation.Id ?? 0;
            addedTransaction.TransactionType = Enums.TransactionTypeCatalog.ReStage;
            return new List<Transaction> { removedTransaction, addedTransaction };
        }

        private Transaction GetRemoveInventoryTransactionsModel(RemoveInventoryItemsListViewModel item, long id)
        {
            var removedTransaction = _mapper.Map<Transaction>(item);
            removedTransaction.EntityId = id;
            removedTransaction.Quantity = item.RemovedQuantity * -1;
            removedTransaction.TransactionType = Enums.TransactionTypeCatalog.Removed;
            return removedTransaction;

        }

        private Transaction GetReturnInventoryTransactionsModel(ReturnInventoryItemsListViewModel item)
        {
            var returnedTransaction = _mapper.Map<Transaction>(item);
            returnedTransaction.Quantity = item.ReturnedQuantity;
            returnedTransaction.TransactionType = Enums.TransactionTypeCatalog.Return;
            return returnedTransaction;

        }


    }
}
