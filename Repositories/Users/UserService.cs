using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Users;
using Repositories.Common.Users.Interface;
using Repositories.Services.AuthenticationService.Interface;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Repositories.Interfaces;
using ViewModels.User.interfaces;
using Enums;
using ViewModels.Role;
using DataLibrary.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Common.Users
{
    public class UserService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        UserBaseService<ApplicationUser, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IUserService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        public IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel> ProfileService { get; set; }

        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public UserService(
            IActionContextAccessor actionContext,
            ApplicationDbContext db,
            ILogger<UserService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IIdentityService identity,
            IRepositoryResponse response
            ) : base(actionContext, identity, db, logger, mapper, response, "")
        {
            _db = db;
            _logger = logger;
        }

        public override async Task<Expression<Func<ApplicationUser, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as UserSearchViewModel;

            return x =>
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                || x.FirstName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                || x.LastName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                || x.Email.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )

                        ;
        }
        public override IQueryable<ApplicationUser> GetPaginationDbSet()
        {
            return (from user in _db.Users
                    join userRole in _db.UserRoles on user.Id equals userRole.UserId
                    join r in _db.Roles on userRole.RoleId equals r.Id
                    where r.Name != RolesCatalog.SystemAdministrator.ToString()
                    select user
                    );
        }
        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            var userRoles = await (from r in _db.Roles
                                   join ur in _db.UserRoles on r.Id equals ur.RoleId
                                   join u in _db.Users on ur.UserId equals u.Id
                                   select new
                                   {
                                       RoleId = r.Id,
                                       RoleName = r.Name,
                                       UserId = ur.UserId,
                                   }).ToListAsync();
            var userResponse = await base.GetAll<M>(search);
            var userDetailResponse = userResponse as RepositoryResponseWithModel<PaginatedResultModel<UserDetailViewModel>>;
            foreach (var userDetail in userDetailResponse.ReturnModel.Items)
            {
                var userRole = userRoles.Where(x => x.UserId == userDetail.Id).FirstOrDefault();
                userDetail.Role = new RoleBriefViewModel { Id = userRole.RoleId, Name = userRole.RoleName };
            }
            return userResponse;
        }

        public override async Task<IRepositoryResponse> GetById(long id)
        {
            var userReposne = await base.GetById(id);
            var userDetailResponse = userReposne as RepositoryResponseWithModel<UserDetailViewModel>;
            userDetailResponse.ReturnModel.Role = await (from r in _db.Roles
                                                         join ur in _db.UserRoles on r.Id equals ur.RoleId
                                                         where ur.UserId == userDetailResponse.ReturnModel.Id
                                                         select new RoleBriefViewModel
                                                         {
                                                             Id = r.Id,
                                                             Name = r.Name,
                                                         }).FirstOrDefaultAsync();
            return userReposne;
        }




    }
}

