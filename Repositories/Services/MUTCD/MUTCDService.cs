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
using Helpers.File;

namespace Repositories.Common
{
    public class MUTCDService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<MUTCD, CreateViewModel, UpdateViewModel, DetailViewModel>, IMUTCDService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly IFileHelper _fileHelper;

        public MUTCDService(ApplicationDbContext db, ILogger<MUTCDService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IFileHelper fileHelper, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _fileHelper = fileHelper;
        }

        public override async Task<Expression<Func<MUTCD, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as MUTCDSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Code.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (string.IsNullOrEmpty(searchFilters.Code) || x.Code.ToLower().Contains(searchFilters.Code.ToLower()))
                        ;
        }

        public override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as MUTCDModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            return base.Create(model);
        }

        public override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var viewModel = model as MUTCDModifyViewModel;
            if (viewModel.File != null)
            {
                viewModel.ImageUrl = _fileHelper.Save(viewModel);
            }
            return base.Update(model);
        }

    }
}

