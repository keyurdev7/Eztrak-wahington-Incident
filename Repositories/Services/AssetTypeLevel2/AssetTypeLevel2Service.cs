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
    public class AssetTypeLevel2Service<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<AssetTypeLevel2, CreateViewModel, UpdateViewModel, DetailViewModel>, IAssetTypeLevel2Service<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public AssetTypeLevel2Service(ApplicationDbContext db, ILogger<AssetTypeLevel2Service<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as AssetTypeLevel2ModifyViewModel;
            long assetTypeLevel1Id = _db.AssetTypeLevels1
                                   .Where(a => a.Id == viewModel.AssetTypeLevel1.Id)
                                   .Select(a => a.AssetTypeId)
                                   .FirstOrDefault();
           viewModel.AssetTypeId = assetTypeLevel1Id;
            return base.Create(model);
        }
        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                IQueryable<AssetTypeLevel2DetailViewModel> assetTypeLevel2Queryable = (from assetTypeLevel2 in _db.AssetTypeLevels2
                                                                                       join assetTypeLevel1 in _db.AssetTypeLevels1 on assetTypeLevel2.AssetTypeLevel1Id equals assetTypeLevel1.Id
                                                                                       join assetType in _db.AssetTypes on assetTypeLevel2.AssetTypeId equals assetType.Id
                                                                                       where
                                                                                       (
                                                                                           (string.IsNullOrEmpty(search.Search.value) || assetTypeLevel2.Name.ToLower().Contains(search.Search.value.ToLower()))
                                                                                       // Add more conditions if needed
                                                                                       )
                                                                                       select new AssetTypeLevel2DetailViewModel
                                                                                       {
                                                                                           Id = assetTypeLevel2.Id,
                                                                                           Name = assetTypeLevel2.Name,
                                                                                           AssetTypeLevel1Name= assetTypeLevel1.Name,
                                                                                           // Add more properties if needed
                                                                                       }).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<AssetTypeLevel2DetailViewModel>>();
                responseModel.ReturnModel = await assetTypeLevel2Queryable.Paginate(search);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AssetTypeLevel2Service GetAll method threw an exception, Message: {ex.Message}");
                return new RepositoryResponseWithModel<PaginatedResultModel<AssetTypeLevel2DetailViewModel>>();
            }
        }

        public override async Task<Expression<Func<AssetTypeLevel2, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as AssetTypeLevel2SearchViewModel;

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

    }
}

