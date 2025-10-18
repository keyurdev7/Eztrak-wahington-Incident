using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IEquipmentFieldsService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<EquipmentFieldsModifyViewModel>> GetAllEquipmentFields();
        Task<long> SaveEquipmentFields(EquipmentFieldsModifyViewModel viewModel);
        Task<long> UpdateEquipmentFields(EquipmentFieldsModifyViewModel viewModel);
        Task<EquipmentFieldsModifyViewModel> GetEquipmentFieldsById(long Id);
        Task<long> DeleteEquipmentFields(long id);
    }
}
