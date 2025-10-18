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
using Centangle.Common.ResponseHelpers;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Common
{
    public class UserSearchSettingService<CreateViewModel, UpdateViewModel, DetailViewModel>
            :
            BaseService<UserSearchSetting, CreateViewModel, UpdateViewModel, DetailViewModel>
            , IUserSearchSettingService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, IName, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserSearchSettingService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;

        public UserSearchSettingService(
            ApplicationDbContext db
            , ILogger<UserSearchSettingService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger
            , IMapper mapper
            , IRepositoryResponse response
            , IActionContextAccessor actionContext
            ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public override async Task<Expression<Func<UserSearchSetting, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as UserSearchSettingSearchViewModel;

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
                        &&
                        (searchFilters.Type == null || x.Type == searchFilters.Type)
                        ;
        }

        public override async Task<IRepositoryResponse> Delete(long id)
        {
            try
            {
                if (await CanDelete(id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var dbModel = await _db.UserSearchSettings.FindAsync(id);
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    await _db.SaveChangesAsync();
                    return _response;
                }
                _logger.LogWarning($"No record found for id:{id} for UserSearchSetting in Delete()");

                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete() for UserSearchSetting threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            try
            {
                if (await CanUpdate(model.Id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var updateModel = model as BaseUpdateVM;
                if (updateModel != null)
                {
                    var searchName = model.Name.Trim().ToLower();
                    var record = await _db.UserSearchSettings.Where(x => x.Name.Trim().ToLower().Equals(searchName)).FirstOrDefaultAsync() ?? new();
                    if (record.Id > 0)
                    {
                        model.Id = record.Id;
                        _mapper.Map(model, record);
                    }
                    else
                    {
                        record = _mapper.Map<UserSearchSetting>(model);
                        await _db.AddAsync(record);
                    }
                    await _db.SaveChangesAsync();
                    var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                    return response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update() for UserSearchSettings threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }
    }
}

