using AutoMapper;
using DataLibrary;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Centangle.Common.ResponseHelpers;
using Repositories.Services.AuthenticationService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ViewModels.User.interfaces;
using Microsoft.EntityFrameworkCore;
using ViewModels.Employee;
using Helpers.Extensions;
using Pagination;
using Models;
using Enums;
using ViewModels.Role;
using Microsoft.AspNetCore.Identity;
using ViewModels.Authentication;

namespace Repositories.Common
{
    public abstract class UserBaseService<TEntity, CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<TEntity, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IUserBaseService<CreateViewModel, UpdateViewModel, DetailViewModel> where TEntity : class, IBaseModel, new()
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly IIdentityService _identity;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ModelStateDictionary _modelState;
        private readonly IRepositoryResponse _response;
        private readonly string _role;

        public UserBaseService(
            IActionContextAccessor actionContext,
            IIdentityService identity,
            ApplicationDbContext db,
            ILogger logger,
            IMapper mapper,
            IRepositoryResponse response,
            string role
            ) : base(db, logger, mapper, response)
        {
            _identity = identity;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _role = role;
            _modelState = actionContext.ActionContext.ModelState;
        }

        public async Task<bool> IsEmailUnique(long id, string email)
        {
            return await _identity.IsEmailUnique(id, email);
        }
        public async Task<bool> IsPinCodeUnique(long id, string pinCode)
        {
            var encodedPinCode = pinCode.EncodePasswordToBase64();
            if (pinCode == "9999")
                return false;
            bool isUnique = (await _db.Users.Where(x => x.PinCode == encodedPinCode && x.Id != id && x.IsDeleted == false).CountAsync()) < 1;
            return isUnique;
        }

        public async Task<IRepositoryResponse> ResetPinCode(ChangePinCodeVM model)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                user.PinCode = model.PinCode.EncodePasswordToBase64();
                await _db.SaveChangesAsync();
                var response = new RepositoryResponseWithModel<bool> { ReturnModel = true };
                return response;
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }

        }
        public async Task<List<RoleBriefViewModel>> GetUserRolesForUpdation()
        {
            var allowedRoles = new List<string>
            {
                RolesCatalog.SuperAdministrator.ToString(),
                RolesCatalog.Guest.ToString(),
                RolesCatalog.Manager.ToString(),
                RolesCatalog.Technician.ToString()
            };
            return await (from r in _db.Roles
                          where allowedRoles.Contains(r.Name)
                          select new RoleBriefViewModel()
                          {
                              Id = r.Id,
                              Name = r.Name,
                          }).ToListAsync();
        }

        public override async Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                IRepositoryResponse profileResult = new RepositoryResponse();
                var result = await _identity.CreateUser(model, _modelState);
                if (result != null && result.Status == System.Net.HttpStatusCode.OK && profileResult.Status == System.Net.HttpStatusCode.OK)
                {
                    var parsedResponse = result as RepositoryResponseWithModel<long>;
                    var userId = parsedResponse?.ReturnModel ?? 0;
                    if (model.HasAdditionInfo)
                    {
                        model.UserId = userId;
                        profileResult = await base.Create(model);
                    }

                    await transaction.CommitAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Create method of User Creation ");
            }
            await transaction.RollbackAsync();
            return Response.BadRequestResponse(_response);

        }

        public override async Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                IRepositoryResponse profileResult = new RepositoryResponse();
                IRepositoryResponse result = await UpdateIdentityUser(model);
                if (result != null && result.Status == System.Net.HttpStatusCode.OK && profileResult.Status == System.Net.HttpStatusCode.OK)
                {
                    if (model.HasAdditionInfo)
                    {
                        profileResult = await base.Update(model);
                    }
                    await transaction.CommitAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Create method of User Creation ");
            }
            await transaction.RollbackAsync();
            return Response.BadRequestResponse(_response);

        }

        public async virtual Task<IRepositoryResponse> UpdateIdentityUser(UpdateViewModel model)
        {
            return await _identity.UpdateUser(model, _modelState);
        }

        public override async Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (user != null)
                {
                    var result = _mapper.Map<DetailViewModel>(user);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for Employee in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for Employee threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        //public IQueryable<ApplicationUser> GetPaginationDbSet()
        //{
        //    return (from user in _db.Users
        //            join userRole in _db.UserRoles on user.Id equals userRole.UserId
        //            join r in _db.Roles on userRole.RoleId equals r.Id
        //            where string.IsNullOrEmpty(_role) || r.Name == _role.ToString()
        //            select user
        //            );
        //}
        //public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        //{
        //    try
        //    {
        //        var filters = SetQueryFilter(search);
        //        var result = await GetPaginationDbSet().Paginate(search);
        //        if (result != null)
        //        {
        //            var paginatedResult = new PaginatedResultModel<M>();
        //            paginatedResult.Items = _mapper.Map<List<M>>(result.Items.ToList());
        //            paginatedResult._meta = result._meta;
        //            paginatedResult._links = result._links;
        //            var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
        //            return response;
        //        }
        //        _logger.LogWarning($"No record found for {typeof(TEntity).FullName} in GetAll()");
        //        return Response.NotFoundResponse(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"UserBaseService GetAll method threw an exception, Message: {ex.Message}");
        //        return new RepositoryResponseWithModel<PaginatedResultModel<EmployeeDetailViewModel>>();
        //    }
        //}
    }
}
