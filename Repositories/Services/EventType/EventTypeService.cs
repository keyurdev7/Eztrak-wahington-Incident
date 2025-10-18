using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using System.Linq.Expressions;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class EventTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<EventType, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IEventTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EventTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        public EventTypeService(
            ApplicationDbContext db,
            ILogger<EventTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            )
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        //public override async Task<Expression<Func<EventType, bool>>> SetQueryFilter(IBaseSearchModel filters)
        //{
        //    var searchFilters = filters as EventTypeSearchViewModel;

        //    return x =>
        //                (
        //                    (
        //                        string.IsNullOrEmpty(searchFilters.Search.value)
        //                        ||
        //                        x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                        ||
        //                        x.Description.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                    )
        //                )
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Description) || x.Description.ToLower().Contains(searchFilters.Description.ToLower()))
        //                ;
        //}

        public async Task<List<EventTypeModifyViewModel>> GetAllEventTypes()
        {
            List<EventTypeModifyViewModel> eventTypes = new();
            try
            {
                var eventTypesList = await _db.EventTypes.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var eventType in eventTypesList)
                {
                    eventTypes.Add(new EventTypeModifyViewModel()
                    {
                        Id = eventType.Id,
                        Name = eventType.Name,
                        Description = eventType.Description
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllEventTypes.");
                return new List<EventTypeModifyViewModel>()!;
            }
            return eventTypes;
        }
        public async Task<long> SaveEventType(EventTypeModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var eventType = new EventType
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };

                // Save
                await _db.EventTypes.AddAsync(eventType);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return eventType.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveEventType.");
                return 0;
            }
        }
        public async Task<long> UpdateEventType(EventTypeModifyViewModel viewModel)
        {
            try
            {

                var eventType = await _db.EventTypes.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (eventType == null)
                {
                    await SaveEventType(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                eventType.Name = viewModel.Name;
                eventType.Description = viewModel.Description;

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return eventType.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateEventType.");
                return 0;
            }
        }
        public async Task<EventTypeModifyViewModel> GetEventTypeById(long Id)
        {
            var eventTypeView = new EventTypeModifyViewModel();

            try
            {
                var eventType = await _db.EventTypes.FirstOrDefaultAsync(p => p.Id == Id);

                if (eventType == null)
                {
                    return new EventTypeModifyViewModel();
                }

                eventTypeView.Name = eventType.Name;
                eventTypeView.Description = eventType.Description;
                eventTypeView.Id = eventType.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetEventTypeById.");
                return new EventTypeModifyViewModel();
            }

            return eventTypeView;
        }
        public async Task<long> DeleteEventType(long Id)
        {
            try
            {

                var eventType = await _db.EventTypes.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (eventType == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                eventType.IsDeleted = true;

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return eventType.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteEventType.");
                return 0;
            }
        }
    }
}
