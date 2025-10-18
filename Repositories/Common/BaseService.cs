using AutoMapper;
using DataLibrary;
using Helpers.Extensions;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using System.Linq.Expressions;
using ViewModels.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Centangle.Common.ResponseHelpers;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Common
{
    public abstract class BaseService<TEntity, CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where TEntity : class, IBaseModel, new()
        where DetailViewModel : class, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;

        public BaseService(ApplicationDbContext db, ILogger logger, IMapper mapper, IRepositoryResponse response)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public virtual async Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                if (await CanView(id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var model = await _db.Set<TEntity>().FindAsync(id);
                if (model != null)
                {
                    foreach (var path in GetIncludeColumns())
                    {
                        try
                        {
                            _db.Entry(model).Reference(path).Load();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    var result = _mapper.Map<DetailViewModel>(model);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(TEntity).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(TEntity).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }
        public virtual async Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            try
            {
                if (await CanCreate() == false)
                {
                    return UnAuthorizedResponse();
                }
                var mappedModel = _mapper.Map<TEntity>(model);
                await _db.Set<TEntity>().AddAsync(mappedModel);
                await _db.SaveChangesAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = mappedModel.Id };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Create method of {typeof(TEntity).FullName}");
                return Response.BadRequestResponse(_response);
            }
        }
        public virtual async Task<IRepositoryResponse> Update(UpdateViewModel model)
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
                    var record = await _db.Set<TEntity>().FindAsync(updateModel?.Id);
                    if (record != null)
                    {
                        var dbModel = _mapper.Map(model, record);
                        await _db.SaveChangesAsync();
                        var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                        return response;
                    }
                    _logger.LogWarning($"Record for id: {updateModel?.Id} not found in {typeof(TEntity).FullName} in Update()");
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update() for {typeof(TEntity).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }
        public virtual async Task<IRepositoryResponse> Delete(long id)
        {
            try
            {
                if (await CanDelete(id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var dbModel = await _db.Set<TEntity>().FindAsync(id);
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    await _db.SaveChangesAsync();
                    return _response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(TEntity).FullName} in Delete()");

                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete() for {typeof(TEntity).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }
        public virtual async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            return await GetAll<M>(search, GetIncludeColumns());
        }
        public virtual async Task<IRepositoryResponse> GetSelect2<M>(IBaseSearchModel search)
        {
            return await GetAll<M>(search, GetSelect2IncludeColumns());
        }
        internal virtual List<string> GetIncludeColumns() => new List<string>();
        internal virtual List<string> GetSelect2IncludeColumns()
        {
            return GetIncludeColumns();
        }
        private async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search, List<string> includeColumns)
        {
            try
            {
                IQueryable<TEntity> dbQuery = await GetFilteredQuery(search, includeColumns);
                return await GetPaginatedResult<M, TEntity>(search, dbQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(TEntity).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        public virtual IQueryable<TEntity> GetPaginationDbSet()
        {
            return _db.Set<TEntity>().AsQueryable();
        }
        public virtual async Task<Expression<Func<TEntity, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            return p => p.IsDeleted == null || p.IsDeleted == false;
        }
        protected async Task<IQueryable<TEntity>> GetFilteredQuery(IBaseSearchModel search, List<string> includeColumns)
        {
            var filters = await SetQueryFilter(search);
            var dbQuery = GetPaginationDbSet();
            foreach (var includeColumn in includeColumns)
            {
                dbQuery = dbQuery.Include(includeColumn).AsQueryable();
            }

            dbQuery = dbQuery.Where(filters);
            return dbQuery;
        }

        protected async Task<IRepositoryResponse> GetPaginatedResult<M, E>(IBaseSearchModel search, IQueryable<E> finalQuery, bool ignoreMapping = false)
        {
            var result = await finalQuery.Paginate(search);
            if (result != null)
            {
                var paginatedResult = new PaginatedResultModel<M>();
                if (ignoreMapping)
                {
                    paginatedResult.Items = result.Items as List<M>;
                }
                else
                {
                    paginatedResult.Items = _mapper.Map<List<M>>(result.Items.ToList());
                }
                paginatedResult._meta = result._meta;
                paginatedResult._links = result._links;
                var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                return response;
            }
            _logger.LogWarning($"No record found for {typeof(TEntity).FullName} in GetAll()");
            return Response.NotFoundResponse(_response);
        }

        protected virtual Task<bool> CanView(long id)
        {
            return Task.FromResult(true);
        }
        protected virtual Task<bool> CanCreate()
        {
            return Task.FromResult(true);
        }
        protected virtual Task<bool> CanUpdate(long id)
        {
            return Task.FromResult(true);
        }
        protected virtual Task<bool> CanDelete(long id)
        {
            return Task.FromResult(true);
        }

        protected IRepositoryResponse UnAuthorizedResponse(string? message = null)
        {
            _response.Message = message ?? "You are not authoirzed to perform this action!";
            return Response.BadRequestResponse(_response);
        }

    }
}
