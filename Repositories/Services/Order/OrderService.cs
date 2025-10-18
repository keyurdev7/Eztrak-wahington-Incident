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
using Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.Users;
using Helpers.Extensions;

namespace Repositories.Common
{
    public class OrderService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Order, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IOrderService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : OrderDetailViewModel, IBaseCrudViewModel, new()
        where CreateViewModel : OrderModifyViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<OrderService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IActionContextAccessor _actionContext;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly IEquipmentTransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _eqiupmentTransactionService;
        private readonly IUserInfoService _userInfo;

        public OrderService(
            ApplicationDbContext db,
            ILogger<OrderService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            , ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService
            , IEquipmentTransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> eqiupmentTransactionService
            , IUserInfoService userInfo
            )
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _actionContext = actionContext;
            _transactionService = transactionService;
            this._eqiupmentTransactionService = eqiupmentTransactionService;
            this._userInfo = userInfo;
        }

        public override async Task<Expression<Func<Order, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as OrderSearchViewModel;

            return x =>
                        //(
                        //    (
                        //        string.IsNullOrEmpty(searchFilters.Search.value)
                        //        ||
                        //        x.WorkOrder.Name.ToLower().Contains(searchFilters.Search.value.ToLower())

                        //    )
                        //)
                        //&&
                        (searchFilters.WorkOrder.Id == null || x.WorkOrderId == searchFilters.WorkOrder.Id)
                        ;
        }
        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var searchFilters = search as OrderSearchViewModel;
                var userRole = _userInfo.LoggedInUserRole();
                var isUserTechnician = userRole == RolesCatalog.Technician.ToString();
                var isUserManager = userRole == RolesCatalog.Manager.ToString();
                var loggedInUserId = long.Parse(_userInfo.LoggedInUserId());
                var orders = (from
                              o in _db.Orders
                              join w in _db.WorkOrder
                                          .Include(x => x.Asset)
                                          .Include(x => x.Manager)
                                          .Include(x => x.Technicians)
                                        on o.WorkOrderId equals w.Id into wl
                              from w in wl.DefaultIfEmpty()
                              join r in _db.Users on o.CreatedBy equals r.Id
                              //let attachments = _db.Attachments
                              //                    .Where(att => att.EntityType == Enums.AttachmentEntityType.WorkOrders && att.EntityId == w.Id)
                              //                    .Select(att => new AttachmentVM
                              //                    {
                              //                        Id = att.Id,
                              //                        Name = att.Name,
                              //                        Url = att.Url,
                              //                        EntityId = (long)att.EntityId
                              //                    }).ToList()
                              where
                              (
                                  (
                                      string.IsNullOrEmpty(searchFilters.Search.value)
                                      ||
                                      (
                                          w == null
                                          ||
                                          w.Asset.SystemGeneratedId.ToString().ToLower().Contains(searchFilters.Search.value.ToLower())
                                          ||
                                          w.Manager.FirstName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                      )
                                  )
                                  &&
                                  (isUserManager == false || w.ManagerId == loggedInUserId)
                                  &&
                                  (isUserTechnician == false || w.Technicians.Any(x => x.TechnicianId == loggedInUserId))
                              )
                              select new OrderDetailViewModel
                              {
                                  Id = o.Id,
                                  Type = o.Type,
                                  WorkOrder = w != null ? new WorkOrderDetailViewModel
                                  {
                                      Id = w.Id,
                                      Status = w.Status,
                                      SystemGeneratedId = w.SystemGeneratedId,
                                      //AssetStreet = w.Asset.Intersection,
                                      Urgency = w.Urgency,
                                  } : new WorkOrderDetailViewModel(),
                                  OrderNumber = o.OrderNumber,
                                  TotalCost = o.Cost ?? 0,
                                  Requestor = new UserBriefViewModel
                                  {
                                      Id = r.Id,
                                      FirstName = r.FirstName,
                                      LastName = r.LastName
                                  },
                                  Notes = o.Notes,
                                  Status = o.Status,
                                  CreatedOn = o.CreatedOn
                              }).AsQueryable();

                var result = await orders.Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<M>();
                    paginatedResult.Items = _mapper.Map<List<M>>(result.Items);
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(WorkOrder).FullName} in GetAll()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(WorkOrder).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "WorkOrder", "OrderItems" };
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await CanCreate() == false)
                {
                    return UnAuthorizedResponse();
                }
                var mappedModel = _mapper.Map<Order>(model);
                var currentCount = await _db.Orders.IgnoreQueryFilters().CountAsync();
                mappedModel.OrderNumber = "OD-" + (currentCount + 1).ToString("D4");
                mappedModel.Status = OrderStatus.Submitted;
                await _db.AddAsync(mappedModel);
                await _db.SaveChangesAsync();
                var orderItems = _mapper.Map<List<OrderItem>>(model.OrderItems);
                foreach (var i in orderItems)
                {
                    i.OrderId = mappedModel.Id;
                }
                orderItems.ForEach(x => x.OrderId = mappedModel.Id);
                await _db.AddRangeAsync(orderItems);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = mappedModel.Id };
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Exception thrown in Create method of Order");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                if (await CanView(id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var orderQueryable = _db.Orders
                    .Include(x => x.OrderItems).ThenInclude(x => x.Inventory).ThenInclude(x => x.UOM)
                    .Include(x => x.OrderItems).ThenInclude(x => x.Inventory).ThenInclude(x => x.Manufacturer)
                    .Include(x => x.OrderItems).ThenInclude(x => x.Equipment).ThenInclude(x => x.UOM)
                    .Include(x => x.OrderItems).ThenInclude(x => x.Equipment).ThenInclude(x => x.Manufacturer)
                    .Where(x => x.Id == id);
                var query = orderQueryable.ToQueryString();
                var model = await orderQueryable.AsNoTracking().FirstOrDefaultAsync();

                if (model != null)
                {
                    var result = _mapper.Map<DetailViewModel>(model);
                    var inventoryIds = result.OrderItems.Where(x => x.Inventory != null && x.Inventory.Id > 0).Select(x => x.Inventory.Id).ToList();
                    var transactions = await _transactionService.GetGroupedTransactionsByItemsForOrder(inventoryIds);
                    foreach (var item in result.OrderItems)
                    {
                        var transaction = transactions.Where(x => x.Inventory.Id == item.Inventory.Id).FirstOrDefault();
                        if (transaction != null)
                        {
                            item.OHQuantity = (long)transaction.Quantity;
                        }
                    }
                    var equipmentIds = result.OrderItems.Where(x => x.Equipment != null && x.Equipment.Id > 0).Select(x => x.Equipment.Id).ToList();
                    var equipmentTransaction = await _eqiupmentTransactionService.GetGroupedTransactionsByItemsForOrder(equipmentIds);
                    foreach (var item in result.OrderItems)
                    {
                        var transaction = equipmentTransaction.Where(x => x.Equipment.Id == item.Equipment.Id).FirstOrDefault();
                        if (transaction != null)
                        {
                            item.OHQuantity = (long)transaction.Quantity;
                        }
                    }
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for Orders in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for Orders threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetInventoryToIssue(long orderItemId, long inventoryId)
        {
            try
            {
                var transactions = await _transactionService.GetGroupedTransactionsByItems(new List<long> { inventoryId });
                var dbOrder = await _db.OrderItems.Include(x => x.Order).Include(x => x.Inventory).Where(x => x.Id == orderItemId).AsNoTracking().FirstOrDefaultAsync();
                if (dbOrder != null)
                {
                    var order = _mapper.Map<OrderItemDetailViewModel>(dbOrder);

                    var result = new IssueInventoryItemViewModel
                    {
                        OrderItem = order,
                        Inventory = order.Inventory,
                        Transactions = _mapper.Map<List<IssueInventoryItemListViewModel>>(transactions)

                    };
                    var response = new RepositoryResponseWithModel<IssueInventoryItemViewModel> { ReturnModel = result };
                    return response;
                }

                _logger.LogWarning($"No record found for id:{orderItemId} and inventoryId:{inventoryId} for Orders in GetInventoryToIssue()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for Orders threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetEquipmentToIssue(long orderItemId, long equipmentId)
        {
            try
            {
                var transactions = await _eqiupmentTransactionService.GetGroupedTransactionsByItems(new List<long> { equipmentId });
                var dbOrder = await _db.OrderItems.Include(x => x.Order).Include(x => x.Equipment).Where(x => x.Id == orderItemId).AsNoTracking().FirstOrDefaultAsync();
                if (dbOrder != null)
                {
                    var order = _mapper.Map<OrderItemDetailViewModel>(dbOrder);

                    var result = new IssueEquipmentItemViewModel
                    {
                        OrderItem = order,
                        Equipment = order.Equipment,
                        Transactions = _mapper.Map<List<IssueEquipmentItemListViewModel>>(transactions)

                    };
                    var response = new RepositoryResponseWithModel<IssueEquipmentItemViewModel> { ReturnModel = result };
                    return response;
                }

                _logger.LogWarning($"No record found for id:{orderItemId} and equipmentId:{equipmentId} for Orders in GetEquipmentToIssue()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for Orders threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> IssueInventoryItem(IssueInventoryItemViewModel model)
        {
            var dbTransaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var orderItem = await _db.OrderItems.Where(x => x.Id == model.OrderItem.Id).Include(x => x.Order).FirstOrDefaultAsync();
                if (orderItem != null)
                {
                    var targetTransactions = model.Transactions.Where(x => x.Quantity > 0).ToList();
                    foreach (var transaction in targetTransactions)
                    {
                        var trans = GetIssueTransactionsModel(transaction, orderItem?.OrderId ?? 0, orderItem?.Id ?? 0);
                        _db.Transactions.Add(trans);
                    }
                    orderItem.IsIssued = true;
                    orderItem.Order.Status = OrderStatus.Issued;
                    await _db.SaveChangesAsync();

                    //add it to WorkOrder cost
                    var workOrder = await (from wo in _db.WorkOrder
                                           join o in _db.Orders on wo.Id equals o.WorkOrderId
                                           where o.Id == orderItem.OrderId
                                           select wo
                                           ).FirstOrDefaultAsync();
                    if (workOrder != null)
                    {
                        var assetCost = await GetOrderItemTotalCost(orderItem.Id);
                        workOrder.MaterialCost += assetCost;
                        workOrder.ActualCost += assetCost;
                    }
                    var areAllOrdersIssued = (await _db.OrderItems.Where(x => x.IsIssued == false).CountAsync()) == 0;
                    if (areAllOrdersIssued)
                    {
                        orderItem.Order.Status = OrderStatus.Delivered;
                    }
                    await _db.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return _response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, ex.Message);
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> IssueEquipmentItem(IssueEquipmentItemViewModel model)
        {
            var dbTransaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var orderItem = await _db.OrderItems.Where(x => x.Id == model.OrderItem.Id).Include(x => x.Order).FirstOrDefaultAsync();
                if (orderItem != null)
                {
                    var targetTransactions = model.Transactions.Where(x => x.Quantity > 0).ToList();
                    foreach (var transaction in targetTransactions)
                    {
                        _db.EquipmentTransactions.Add(GetIssueEquipmentTransactionsModel(transaction, orderItem?.OrderId ?? 0, orderItem?.Id ?? 0));
                    }
                    orderItem.IsIssued = true;
                    orderItem.Order.Status = OrderStatus.Issued;
                    await _db.SaveChangesAsync();
                    var areAllOrdersIssued = (await _db.OrderItems.Where(x => x.IsIssued == false).CountAsync()) == 0;
                    if (areAllOrdersIssued)
                    {
                        orderItem.Order.Status = OrderStatus.Delivered;
                        await _db.SaveChangesAsync();
                    }
                    await dbTransaction.CommitAsync();
                    return _response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, ex.Message);
                return Response.BadRequestResponse(_response);
            }
        }

        private Transaction GetIssueTransactionsModel(IssueInventoryItemListViewModel item, long orderId, long orderItemId)
        {
            var issuedTransaction = _mapper.Map<Transaction>(item);
            issuedTransaction.Quantity = item.Quantity * -1;
            issuedTransaction.TransactionType = TransactionTypeCatalog.Order;
            issuedTransaction.EntityId = orderId;
            issuedTransaction.EntityDetailId = orderItemId;
            return issuedTransaction;
        }

        private EquipmentTransaction GetIssueEquipmentTransactionsModel(IssueEquipmentItemListViewModel item, long orderId, long orderItemId)
        {
            var issuedTransaction = _mapper.Map<EquipmentTransaction>(item);
            issuedTransaction.Quantity = item.Quantity * -1;
            issuedTransaction.TransactionType = EquipmentTransactionTypeCatalog.Order;
            issuedTransaction.EntityId = orderId;
            issuedTransaction.EntityDetailId = orderItemId;
            return issuedTransaction;
        }

        public async Task<IRepositoryResponse> Submit(long id, OrderTypeCatalog type)
        {
            var order = await _db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order != null)
            {
                var workOrder = await _db.WorkOrder.FirstOrDefaultAsync(x => x.Id == order.WorkOrderId);
                if (workOrder != null)
                {
                    var _transaction = await _db.Database.BeginTransactionAsync();
                    try
                    {
                        var orderTotalCost = await GetOrderTotalCost(order.Id);
                        order.Cost = orderTotalCost;
                        //workOrder.ActualCost += orderTotalCost;

                        order.Type = type;
                        order.Status = OrderStatus.Delivered;
                        await _db.SaveChangesAsync();
                        await _transaction.CommitAsync();
                        return _response;
                    }
                    catch (Exception ex)
                    {
                        await _transaction.RollbackAsync();
                        _logger.LogError(ex, ex.Message);
                        return Response.BadRequestResponse(_response);
                    }
                }
            }
            return Response.NotFoundResponse(_response);
        }

        private async Task<double> GetOrderTotalCost(long orderId)
        {
            var totalCostQueryable = (from oi in _db.OrderItems
                                      join t in _db.Transactions on oi.OrderId equals t.EntityId
                                      where t.TransactionType == TransactionTypeCatalog.Order
                                      && oi.OrderId == orderId
                                      select new
                                      {
                                          Cost = (t.ItemPrice * t.Quantity)
                                      }).AsQueryable();
            var totalCost = await totalCostQueryable.SumAsync(x => x.Cost);
            return Math.Abs(totalCost);
        }

        private async Task<double> GetOrderItemTotalCost(long orderItemId)
        {
            var totalCostQueryable = (from t in _db.Transactions
                                      where t.TransactionType == TransactionTypeCatalog.Order
                                      && t.EntityDetailId == orderItemId
                                      select new
                                      {
                                          Cost = (t.ItemPrice * t.Quantity)
                                      }).AsQueryable();
            var totalCost = await totalCostQueryable.SumAsync(x => x.Cost);
            return Math.Abs(totalCost);
        }

        private async Task<double> GetEquipmentOrderTotalCost(long orderId)
        {
            var totalCostQueryable = (from oi in _db.OrderItems
                                      join t in _db.EquipmentTransactions on oi.OrderId equals t.EntityId
                                      where t.TransactionType == EquipmentTransactionTypeCatalog.Order
                                      && oi.OrderId == orderId
                                      select new
                                      {
                                          Cost = (t.Quantity * t.HourlyRate * t.Hours)
                                      }).AsQueryable();
            var totalCost = await totalCostQueryable.SumAsync(x => x.Cost);
            return Math.Abs(totalCost);
        }
    }
}
