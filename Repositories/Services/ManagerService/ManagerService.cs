using Models.Common.Interfaces;
using ViewModels.Shared;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Manager;
using Models;
using Repositories.Common.ManagerService.Interface;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Services.AuthenticationService.Interface;
using Centangle.Common.ResponseHelpers;
using ViewModels.User.interfaces;
using Repositories.Common.Users.Interface;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using Helpers.Extensions;
using ViewModels.Authentication;
using Enums;
using ViewModels;

namespace Repositories.Common.ManagerService
{
    public class ManagerService<CreateViewModel, UpdateViewModel, DetailViewModel> : UserBaseService<ApplicationUser, CreateViewModel, UpdateViewModel, DetailViewModel>, IManagerService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ModelStateDictionary _modelState;
        private readonly ILogger<ManagerService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identity;
        private readonly IRepositoryResponse _response;

        public ManagerService(
            IActionContextAccessor actionContext,
            ApplicationDbContext db,
            ILogger<ManagerService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IIdentityService identity,
            IRepositoryResponse response) : base(actionContext, identity, db, logger, mapper, response, RolesCatalog.Manager.ToString())
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _identity = identity;
            _response = response;
            _modelState = actionContext.ActionContext.ModelState;
        }

        public override async Task<Expression<Func<ApplicationUser, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as ManagerSearchViewModel;

            return x =>
                            (string.IsNullOrEmpty(searchFilters.Search.value) || x.FirstName.ToLower().Contains(searchFilters.Search.value.ToLower()))
                            &&
                            (string.IsNullOrEmpty(searchFilters.Name) || x.FirstName.ToLower().Contains(searchFilters.Name.ToLower()))
                        ;
        }


        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var userQueryable = (from user in _db.Users
                                     join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                     join r in _db.Roles on userRole.RoleId equals r.Id
                                     where
                                     (
                                         (string.IsNullOrEmpty(search.Search.value) || user.FirstName.ToLower().Contains(search.Search.value.ToLower()))
                                         &&
                                         (r.Name == "Manager")
                                     )
                                     select new ManagerDetailViewModel
                                     {
                                         Id = user.Id,
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         Email = user.Email,
                                     }).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<ManagerDetailViewModel>>();
                responseModel.ReturnModel = await userQueryable.Paginate(search);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ManagerService GetAll method threw an exception, Message: {ex.Message}");
                return new RepositoryResponseWithModel<PaginatedResultModel<ManagerDetailViewModel>>();
            }
        }

        public async Task<IRepositoryResponse> GetDataForSelect2<M>(IBaseSearchModel search)
        {
            try
            {
                var userQueryable = (from user in _db.Users
                                     join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                     join r in _db.Roles on userRole.RoleId equals r.Id
                                     where
                                     (
                                         (string.IsNullOrEmpty(search.Search.value) || user.FirstName.ToLower().Contains(search.Search.value.ToLower()))
                                         &&
                                         (r.Name == "Manager")
                                     )
                                     select new ManagerSelect2ViewModel
                                     {
                                         Id = user.Id,
                                         Select2Text = user.FirstName + " " + user.LastName,
                                     }).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<ManagerSelect2ViewModel>>();
                responseModel.ReturnModel = await userQueryable.Paginate(search);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ManagerService GetAll method threw an exception, Message: {ex.Message}");
                return new RepositoryResponseWithModel<PaginatedResultModel<BaseSelect2VM>>();
            }
        }
    }
}

