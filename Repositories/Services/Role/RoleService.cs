using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using Repositories.Common.Role.Interface;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Role;
using ViewModels.Authentication;
using Centangle.Common.ResponseHelpers;
using Helpers.Extensions;

namespace Repositories.Common.Role
{
    public class RoleService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<ApplicationRole, CreateViewModel, UpdateViewModel, DetailViewModel>, IRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RoleService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;

        public RoleService(ApplicationDbContext db, ILogger<RoleService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response) : base(db, logger, mapper, response)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public override async Task<Expression<Func<ApplicationRole, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as RoleSearchViewModel;

            return x =>
                            (string.IsNullOrEmpty(searchFilters.Search.value) || x.Name.ToLower().Contains(searchFilters.Search.value.ToLower()))
                            &&
                            (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                        ;
        }
        public async Task<IRepositoryResponse> GetRoles<M>(IBaseSearchModel search)
        {
            try
            {
                var filter = search as UserRolesSearchVM;
                var result = await _db.Roles.Where(x =>
                (string.IsNullOrEmpty(filter.Name) || x.Name.ToLower().Trim().Contains(filter.Name))
                ).Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<M>();
                    paginatedResult.Items = _mapper.Map<List<M>>(result.Items.ToList());
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for Roles in GetRoles()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetRoles() method for Roles threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }
    }
}

