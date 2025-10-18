using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using Pagination;
using System.Linq.Expressions;
using ViewModels;
using Models;


namespace Repositories.Common
{
    public class ShipmentService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Transaction, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IShipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ShipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public ShipmentService(ApplicationDbContext db, ILogger<ShipmentService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) :
            base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }


        public virtual Expression<Func<Transaction, bool>> SetQueryFilter(IBaseSearchModel filters)
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

                        && x.TransactionType == TransactionTypeCatalog.Shipment
                        ;
        }

        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "Location", "Source", "Supplier" };
        }


    }
}
