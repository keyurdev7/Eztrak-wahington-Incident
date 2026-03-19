using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Repositories.Services.EventSubType.Interface;
using ViewModels.EventSubType;
using EventSubTypeEntity = Models.EventSubType;

namespace Repositories.Services.EventSubType
{
    public class EventSubTypeService : IEventSubTypeService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EventSubTypeService> _logger;

        public EventSubTypeService(ApplicationDbContext db, ILogger<EventSubTypeService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<EventSubTypeModifyViewModel>> GetAll(long? eventTypeId = null)
        {
            try
            {
                var q = _db.EventSubTypes
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                if (eventTypeId.HasValue && eventTypeId.Value > 0)
                    q = q.Where(x => x.EventTypeId == eventTypeId.Value);

                var rows = await (
                        from st in q
                        join t in _db.EventTypes.AsNoTracking().Where(t => !t.IsDeleted)
                            on st.EventTypeId equals t.Id
                        orderby st.Name
                        select new EventSubTypeModifyViewModel
                        {
                            Id = st.Id,
                            EventTypeId = st.EventTypeId,
                            EventTypeName = t.Name,
                            Name = st.Name,
                            Description = st.Description
                        }
                    ).ToListAsync();

                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAll EventSubTypes.");
                return new List<EventSubTypeModifyViewModel>();
            }
        }

        public async Task<EventSubTypeModifyViewModel> GetById(long id)
        {
            try
            {
                var x = await _db.EventSubTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Id == id);

                if (x == null) return new EventSubTypeModifyViewModel();

                var typeName = await _db.EventTypes
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted && t.Id == x.EventTypeId)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync();

                return new EventSubTypeModifyViewModel
                {
                    Id = x.Id,
                    EventTypeId = x.EventTypeId,
                    EventTypeName = typeName,
                    Name = x.Name,
                    Description = x.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById EventSubType.");
                return new EventSubTypeModifyViewModel();
            }
        }

        public async Task<long> Save(EventSubTypeModifyViewModel vm)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = new EventSubTypeEntity
                {
                    EventTypeId = vm.EventTypeId,
                    Name = vm.Name,
                    Description = vm.Description,
                    IsDeleted = false
                };

                await _db.EventSubTypes.AddAsync(entity);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return entity.Id;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error Save EventSubType.");
                return 0;
            }
        }

        public async Task<long> Update(EventSubTypeModifyViewModel vm)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = await _db.EventSubTypes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == vm.Id);
                if (entity == null) return 0;

                entity.EventTypeId = vm.EventTypeId;
                entity.Name = vm.Name;
                entity.Description = vm.Description;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return entity.Id;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error Update EventSubType.");
                return 0;
            }
        }

        public async Task<long> Delete(long id)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = await _db.EventSubTypes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
                if (entity == null) return 0;

                entity.IsDeleted = true;
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return entity.Id;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error Delete EventSubType.");
                return 0;
            }
        }

        public async Task<List<(long Id, string Name)>> GetForDropdown(long eventTypeId)
        {
            try
            {
                var rows = await _db.EventSubTypes
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && x.EventTypeId == eventTypeId)
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

                return rows.Select(r => (r.Id, r.Name)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetForDropdown EventSubType.");
                return new List<(long, string)>();
            }
        }
    }
}

