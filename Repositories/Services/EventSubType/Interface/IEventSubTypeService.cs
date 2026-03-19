using ViewModels.EventSubType;

namespace Repositories.Services.EventSubType.Interface
{
    public interface IEventSubTypeService
    {
        Task<List<EventSubTypeModifyViewModel>> GetAll(long? eventTypeId = null);
        Task<EventSubTypeModifyViewModel> GetById(long id);
        Task<long> Save(EventSubTypeModifyViewModel vm);
        Task<long> Update(EventSubTypeModifyViewModel vm);
        Task<long> Delete(long id);
        Task<List<(long Id, string Name)>> GetForDropdown(long eventTypeId);
    }
}

