using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Helpers.Extensions;

namespace Repositories.Common
{
    public class AssetTypeLevel1Service<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<AssetTypeLevel1, CreateViewModel, UpdateViewModel, DetailViewModel>, IAssetTypeLevel1Service<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public AssetTypeLevel1Service(ApplicationDbContext db, ILogger<AssetTypeLevel1Service<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }
        //public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        //{
        //    try
        //    {
        //        IQueryable<AssetTypeLevel1DetailViewModel> assetTypeLevel2Queryable = (from assetTypeLevel1 in _db.AssetTypeLevels1
        //                                                                               join assetType in _db.AssetTypes on assetTypeLevel1.AssetTypeId equals assetType.Id
        //                                                                               where
        //                                                                               (
        //                                                                                   (string.IsNullOrEmpty(search.Search.value) || assetTypeLevel1.Name.ToLower().Contains(search.Search.value.ToLower()))
        //                                                                               // Add more conditions if needed
        //                                                                               )
        //                                                                               select new AssetTypeLevel1DetailViewModel
        //                                                                               {
        //                                                                                   Id = assetTypeLevel1.Id,
        //                                                                                   Name = assetTypeLevel1.Name,
        //                                                                                   AssetTypeName = assetType.Name,
        //                                                                               }).AsQueryable();

        //        var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<AssetTypeLevel1DetailViewModel>>();
        //        responseModel.ReturnModel = await assetTypeLevel2Queryable.Paginate(search);
        //        return responseModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"AssetTypeLevel2Service GetAll method threw an exception, Message: {ex.Message}");
        //        return new RepositoryResponseWithModel<PaginatedResultModel<AssetTypeLevel1DetailViewModel>>();
        //    }
        //}
        public override async Task<Expression<Func<AssetTypeLevel1, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as AssetTypeLevel1SearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                        ;
        }
        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "AssetType" };
        }

    }
}

