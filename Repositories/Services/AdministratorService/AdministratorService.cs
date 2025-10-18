using Models.Common.Interfaces;
using ViewModels.Shared;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Administrator;
using Models;
using Repositories.Common.AdministratorService.Interface;
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
using Repositories.Common.AdministratorService.Interface;

namespace Repositories.Common.AdministratorService
{
    public class AdministratorService<CreateViewModel, UpdateViewModel, DetailViewModel> : UserBaseService<ApplicationUser, CreateViewModel, UpdateViewModel, DetailViewModel>, IAdministratorService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ModelStateDictionary _modelState;
        private readonly ILogger<AdministratorService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identity;
        private readonly IRepositoryResponse _response;

        public AdministratorService(
            IActionContextAccessor actionContext,
            ApplicationDbContext db,
            ILogger<AdministratorService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IIdentityService identity,
            IRepositoryResponse response) : base(actionContext, identity, db, logger, mapper, response, RolesCatalog.SystemAdministrator.ToString())
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
            var searchFilters = filters as AdministratorSearchViewModel;

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
                IQueryable<AdministratorDetailViewModel> userQueryable = (from user in _db.Users
                                                                          join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                                                          join r in _db.Roles on userRole.RoleId equals r.Id
                                                                          where
                                                                          (
                                                                              (string.IsNullOrEmpty(search.Search.value) || user.FirstName.ToLower().Contains(search.Search.value.ToLower()))
                                                                              &&
                                                                              (r.Name == "SystemAdministrator")
                                                                          )
                                                                          select new AdministratorDetailViewModel
                                                                          {
                                                                              Id = user.Id,
                                                                              FirstName = user.FirstName,
                                                                              LastName = user.LastName,
                                                                              Email = user.Email,
                                                                          }).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<AdministratorDetailViewModel>>();
                responseModel.ReturnModel = await userQueryable.Paginate(search);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AdministratorService GetAll method threw an exception, Message: {ex.Message}");
                return new RepositoryResponseWithModel<PaginatedResultModel<AdministratorDetailViewModel>>();
            }
        }
    }
}

