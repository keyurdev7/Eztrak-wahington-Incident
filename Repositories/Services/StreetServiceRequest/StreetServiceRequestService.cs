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
using Microsoft.EntityFrameworkCore;
using Helpers.File;
using ViewModels.Administrator;

namespace Repositories.Common
{
    public class StreetServiceRequestService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<StreetServiceRequest, CreateViewModel, UpdateViewModel, DetailViewModel>, IStreetServiceRequestService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;

        public StreetServiceRequestService(ApplicationDbContext db, ILogger<StreetServiceRequestService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext, IFileHelper fileHelper) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _mapper = mapper;
            _fileHelper = fileHelper;
        }

        public async Task<List<SSRNotesViewModel>> GetNotesBySSRId(int id)
        {
            try
            {
                var notes = await (from n in _db.StreetServiceRequestNotes.Include(x => x.StreetServiceRequest)
                                   join u in _db.Users on n.CreatedBy equals u.Id
                                   where (n.StreetServiceRequestId == id)
                                   select new SSRNotesViewModel
                                   {
                                       Description = n.Description,
                                       FileUrl = n.FileUrl,
                                       CreatedOn = n.CreatedOn,
                                       CreatedBy = u.FirstName + " " + u.LastName,
                                   }).ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<bool> SaveNotes(SSRNotesViewModel model)
        {
            try
            {
                model.FileUrl = _fileHelper.Save(model);
                var mappedNotes = _mapper.Map<StreetServiceRequestNotes>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            var response = await base.GetAll<M>(search);
            var responseModel = response as RepositoryResponseWithModel<PaginatedResultModel<StreetServiceRequestDetailViewModel>>;
            var notes = await _db.StreetServiceRequestNotes.GroupBy(x => x.StreetServiceRequestId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count > 0).ToListAsync();
            foreach (var item in responseModel.ReturnModel.Items)
            {
                item.HasNotes = notes.Any(x => x.Id == item.Id);
            }
            return response;

        }
        public override async Task<Expression<Func<StreetServiceRequest, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as StreetServiceRequestSearchViewModel;

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

