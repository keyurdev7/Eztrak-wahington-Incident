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

namespace Repositories.Common
{
    public class EquipmentService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Equipment, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IEquipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EquipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IFileHelper _fileHelper;

        public EquipmentService(ApplicationDbContext db, ILogger<EquipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext, IFileHelper fileHelper) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _fileHelper = fileHelper;
        }

        public override async Task<Expression<Func<Equipment, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as EquipmentSearchViewModel;

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
                                x.ItemNo.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (searchFilters.UOM.Id == null || x.UOM.Id == searchFilters.UOM.Id)
                        &&
                        (searchFilters.Category.Id == null || x.Category.Id == searchFilters.Category.Id)
                        &&
                        (searchFilters.Manufacturer.Id == null || x.Manufacturer.Id == searchFilters.Manufacturer.Id)
                        ;
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var dbModel = await _db.Equipments
                                        .Include(x => x.Category)
                                        .Include(x => x.UOM)
                                        .Include(x => x.Manufacturer)
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var result = _mapper.Map<DetailViewModel>(dbModel);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Equipment).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Equipment).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetTransactions(long id)
        {
            try
            {
                var shipments = await _db.EquipmentTransactions
                                        .Include(x => x.Supplier)
                                        .Include(x => x.Location)
                                        .Where(x => x.EquipmentId == id)
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
                var searchModel = search as EquipmentSearchViewModel;
                if (typeof(M) == typeof(EquipmentBriefViewModel))
                {
                    return await base.GetAll<M>(search);
                }
                else
                {

                    var query = await GetAllQuerable(search);
                    var dbQuery = query.GroupBy(x => x.Id).Select(g => new EquipmentDetailViewModel()
                    {
                        Id = g.Key,
                        ImageUrl = g.Max(x => x.ImageUrl),
                        ItemNo = g.Max(x => x.ItemNo),
                        SystemGeneratedId = g.Max(x => x.SystemGeneratedId),
                        Description = g.Max(x => x.Description),
                        Quantity = g.Sum(x => x.Quantity),
                        HourlyRate = g.Max(x => x.HourlyRate),
                        ItemPrice = g.Max(x => x.ItemPrice),
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
                        }
                    })
                    //.Where(x => x.Quantity > 0)
                    .AsQueryable();
                    var paginatedModel = await GetPaginatedResult<M, EquipmentDetailViewModel>(searchModel, dbQuery);
                    var responseModel = paginatedModel as RepositoryResponseWithModel<PaginatedResultModel<EquipmentDetailViewModel>>;
                    var notes = await _db.EquipmentNotes.GroupBy(x => x.EquipmentId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count > 0).ToListAsync();
                    foreach (var item in responseModel.ReturnModel.Items)
                    {
                        item.HasNotes = notes.Any(x => x.Id == item.Id);
                    }
                    return paginatedModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(EquipmentDetailViewModel).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }

        }

        public async Task<double> GetTotalEquipmentPrice(IBaseSearchModel search)
        {
            try
            {
                var query = await GetAllQuerable(search);
                return await query.SumAsync(g => g.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTotalEquipmentPrice() method for {typeof(Equipment).FullName} threw an exception.");
                return 0;
            }

        }

        public override IQueryable<Equipment> GetPaginationDbSet()
        {
            return _db.Equipments
                .Include(x => x.Category)
                .Include(x => x.UOM)
                .Include(x => x.Manufacturer)
                .AsQueryable();
        }

        public async Task<List<EquipmentNotesViewModel>> GetNotesByEquipmentId(int id)
        {
            try
            {
                var notes = await (from n in _db.EquipmentNotes.Include(x => x.Equipment)
                                   join u in _db.Users on n.CreatedBy equals u.Id
                                   where (n.EquipmentId == id)
                                   select new EquipmentNotesViewModel
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

        public async Task<List<EquipmentCostViewModel>> GetEquipmentAverageCost(List<long> ids)
        {
            try
            {
                var EquipmentsCosts = await (from i in _db.Equipments
                                             join t in _db.EquipmentTransactions.Where(x => x.TransactionType == Enums.EquipmentTransactionTypeCatalog.Shipment)
                                               on i.Id equals t.EquipmentId
                                             where (ids.Contains(i.Id))
                                             select new
                                             {
                                                 EquipmentId = i.Id,
                                                 ItemPrice = t.ItemPrice,
                                             }).GroupBy(x => x.EquipmentId)
                                   .Select(x => new EquipmentCostViewModel
                                   {
                                       EquipmentId = x.Key,
                                       AverageCost = x.Average(y => y.ItemPrice)
                                   }).ToListAsync();
                return EquipmentsCosts;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<bool> SaveNotes(EquipmentNotesViewModel model)
        {
            try
            {
                model.FileUrl = _fileHelper.Save(model);
                var mappedNotes = _mapper.Map<EquipmentNotes>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateShipments(EquipmentShipmentGridViewModel model)
        {
            try
            {
                var shipments = _mapper.Map<List<EquipmentTransaction>>(model.EquipmentShipments);
                await _db.AddRangeAsync(shipments);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<IQueryable<EquipmentViewModel>> GetAllQuerable(IBaseSearchModel search)
        {
            var filters = await SetQueryFilter(search);
            return (from e in _db.Equipments.Where(filters)
                    join et in _db.EquipmentTransactions on e.Id equals et.EquipmentId into etl
                    from et in etl.DefaultIfEmpty()
                    join c in _db.Categories on e.CategoryId equals c.Id
                    join m in _db.Manufacturers on e.ManufacturerId equals m.Id
                    join uom in _db.UOMs on e.UOMId equals uom.Id
                    select new EquipmentViewModel
                    {
                        Id = e.Id,
                        ImageUrl = e.ImageUrl,
                        ItemNo = e.ItemNo,
                        SystemGeneratedId = e.SystemGeneratedId,
                        Description = e.Description,
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        ManufacturerId = m.Id,
                        ManufacturerName = m.Name,
                        UOMId = uom.Id,
                        UOMName = uom.Name,
                        HourlyRate = e.HourlyRate,
                        Quantity = et == null ? 0 : et.Quantity,
                        ItemPrice = et == null ? 0 : et.ItemPrice,
                        TotalPrice = et == null ? 0 : (et.Quantity * et.ItemPrice),
                    });
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as EquipmentModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            var totalEquipmentCount = await _db.Equipments.IgnoreQueryFilters().CountAsync();
            viewModel.SystemGeneratedId = "EQP-" + (totalEquipmentCount + 1).ToString("D4");
            return await base.Create(model);
        }

        public override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var viewModel = model as EquipmentModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            return base.Update(model);
        }

        public async Task<List<EquipmentTransactionHistoryViewModel>> GetEquipmentIssueHistory(int equipmentId, string poNumber, int locationId)
        {
            try
            {
                var history = await (from t in _db.EquipmentTransactions
                                     join loc in _db.Locations on t.LocationId equals loc.Id
                                     join u in _db.Users on t.CreatedBy equals u.Id
                                     where t.EquipmentId == equipmentId
                                     && t.IsDeleted == false
                                     && (locationId == 0 || t.LocationId == locationId)
                                     && (string.IsNullOrEmpty(poNumber) || t.PoNo == poNumber)
                                     select new EquipmentTransactionHistoryViewModel
                                     {
                                         Id = t.Id,
                                         EquipmentId = t.EquipmentId,
                                         TransactionType = t.TransactionType,
                                         PoNo = t.PoNo,
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

        public async Task<bool> IsItemNoUnique(long id, string itemNo)
        {
            return (await _db.Equipments.Where(x => x.ItemNo == itemNo && x.Id != id && x.IsDeleted == false).CountAsync()) < 1;
        }
    }
}
