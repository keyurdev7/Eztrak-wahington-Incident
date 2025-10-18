using Models.Common.Interfaces;
using ViewModels.Shared;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Guest;
using Models;
using Repositories.Common.GuestService.Interface;
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

namespace Repositories.Common.GuestService
{
    public class GuestService<CreateViewModel, UpdateViewModel, DetailViewModel> : UserBaseService<ApplicationUser, CreateViewModel, UpdateViewModel, DetailViewModel>, IGuestService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ModelStateDictionary _modelState;
        private readonly ILogger<GuestService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identity;
        private readonly IRepositoryResponse _response;

        public GuestService(
            IActionContextAccessor actionContext,
            ApplicationDbContext db,
            ILogger<GuestService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IIdentityService identity,
            IRepositoryResponse response) : base(actionContext, identity, db, logger, mapper, response, RolesCatalog.Guest.ToString())
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
            var searchFilters = filters as GuestSearchViewModel;

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
                IQueryable<GuestDetailViewModel> userQueryable = (from user in _db.Users
                                                                  join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                                                  join r in _db.Roles on userRole.RoleId equals r.Id
                                                                  where
                                                                  (
                                                                      (string.IsNullOrEmpty(search.Search.value) || user.FirstName.ToLower().Contains(search.Search.value.ToLower()))
                                                                      &&
                                                                      (r.Name == "Guest")
                                                                  )
                                                                  select new GuestDetailViewModel
                                                                  {
                                                                      Id = user.Id,
                                                                      FirstName = user.FirstName,
                                                                      LastName = user.LastName,
                                                                      Email = user.Email,
                                                                  }).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<GuestDetailViewModel>>();
                responseModel.ReturnModel = await userQueryable.Paginate(search);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GuestService GetAll method threw an exception, Message: {ex.Message}");
                return new RepositoryResponseWithModel<PaginatedResultModel<GuestDetailViewModel>>();
            }
        }
    }
}

