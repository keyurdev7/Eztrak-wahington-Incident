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
using Enums;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Common
{
    public class DynamicColumnService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<DynamicColumn, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IDynamicColumnService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DynamicColumnService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;

        public DynamicColumnService(
            ApplicationDbContext db
            , ILogger<DynamicColumnService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger
            , IMapper mapper
            , IRepositoryResponse response
            , IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<Expression<Func<DynamicColumn, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as DynamicColumnSearchViewModel;

            return x =>
                        //(
                        //    (
                        //        string.IsNullOrEmpty(searchFilters.Search.value)
                        //        ||
                        //        x.Code.ToLower().Contains(searchFilters.Search.value.ToLower())
                        //    )
                        //)
                        //&&
                        (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                        &&
                        (searchFilters.Type == null || x.Type == searchFilters.Type)
                        &&
                        (searchFilters.EntityType == null || x.EntityType == searchFilters.EntityType)
                        ;
        }

        public async Task<List<DynamicColumnValueDetailViewModel>> GetDynamicColumns(DynamicColumnEntityType entityType, long entityId)
        {
            try
            {
                var columns = await (from dc in _db.DynamicColumns
                                     join dcv in _db.DynamicColumnValues.Where(x => x.EntityId == entityId) on dc.Id equals dcv.DynamicColumnId into dcvl
                                     from dcv in dcvl.DefaultIfEmpty()
                                     where dc.EntityType == entityType
                                     select
                                    new DynamicColumnValueDetailViewModel
                                    {
                                        Id = dcv != null ? dcv.Id : 0,
                                        EntityId = dcv != null ? dcv.EntityId : entityId,
                                        Value = dcv != null ? dcv.Value : "",
                                        DynamicColumn = new DynamicColumnDetailViewModel
                                        {
                                            Id = dc.Id,
                                            EntityType = dc.EntityType,
                                            Name = dc.Name,
                                            Title = dc.Title,
                                            Type = dc.Type,
                                            Regex = dc.Regex
                                        }
                                    }).ToListAsync();
                return columns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in GetDynamicColumns");
                return new List<DynamicColumnValueDetailViewModel>();
            }
        }

        public async Task<bool> UpdateValues<M>(M model) where M : IDynamicColumns, IIdentitifier
        {
            try
            {
                model.DynamicColumns.ForEach(x => x.EntityId = model.Id);
                var columnList = _mapper.Map<List<DynamicColumnValue>>(model.DynamicColumns);
                //To create
                var columnsToCreate = columnList.Where(x => x.Id < 1).ToList();
                if (columnsToCreate.Count > 0)
                {
                    await _db.AddRangeAsync(columnsToCreate);
                }
                var columnsToUpdate = columnList.Where(x => x.Id > 0).ToList();
                if (columnsToUpdate.Count > 0)
                {
                    var updatedColumnIds = columnsToUpdate.Select(x => x.Id).ToList();
                    var existingColumns = await _db.DynamicColumnValues.Where(x => updatedColumnIds.Contains(x.Id)).ToListAsync();
                    foreach (var col in existingColumns)
                    {
                        var modifiedColumn = columnsToUpdate.Where(x => x.Id == col.Id).FirstOrDefault();
                        col.Value = modifiedColumn.Value;
                    }
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateValues in Dynamic Columns threw the following exception");
                return false;
            }
        }

    }
}

