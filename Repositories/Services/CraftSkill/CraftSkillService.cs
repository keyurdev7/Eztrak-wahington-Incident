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
using Helpers.Extensions;
using Centangle.Common.ResponseHelpers;

namespace Repositories.Common
{
    public class CraftSkillService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<CraftSkill, CreateViewModel, UpdateViewModel, DetailViewModel>, ICraftSkillService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CraftSkillService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IRepositoryResponse _response;

        public CraftSkillService(ApplicationDbContext db, ILogger<CraftSkillService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _response = response;
        }
        public async Task<IRepositoryResponse> GetCraftSkillsForSelect2<M>(IBaseSearchModel search)
        {
            try
            {
                var searchFilters = search as CraftSkillSearchViewModel;

                searchFilters.OrderByColumn = string.IsNullOrEmpty(search.OrderByColumn) ? "Id" : search.OrderByColumn;

                var craftSkillsQueryable = (from cs in _db.CraftSkills
                                            where

                                                (string.IsNullOrEmpty(searchFilters.Search.value) || cs.Name.ToLower().Contains(searchFilters.Search.value.ToLower()))
                                                &&
                                                (string.IsNullOrEmpty(searchFilters.Name) || cs.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                                            select new CraftSkillBriefViewModel
                                            {
                                                Id = cs.Id,
                                                Name = cs.Name,
                                                STRate = cs.STRate,
                                                OTRate = cs.OTRate,
                                                DTRate = cs.DTRate
                                            })
                            .AsQueryable();


                var crafts = await craftSkillsQueryable.Paginate(searchFilters);
                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<CraftSkillBriefViewModel>>();
                responseModel.ReturnModel = crafts;
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CraftSkillService GetAll method threw an exception, Message: {ex.Message}");
                return Response.BadRequestResponse(_response);
            }
        }

        public override async Task<Expression<Func<CraftSkill, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as CraftSkillSearchViewModel;

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

