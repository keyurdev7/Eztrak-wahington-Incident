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
    public class MountTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<MountType, CreateViewModel, UpdateViewModel, DetailViewModel>, IMountTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly IFileHelper _fileHelper;

        public MountTypeService(ApplicationDbContext db, ILogger<MountTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IFileHelper fileHelper, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _fileHelper = fileHelper;
        }

        public override async Task<Expression<Func<MountType, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as MountTypeSearchViewModel;

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

