// Repositories/Interfaces/IMaterialService.cs
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

public interface IMaterialService<CreateViewModel, UpdateViewModel, DetailViewModel>
    : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
    where DetailViewModel : class, IBaseCrudViewModel, new()
    where CreateViewModel : class, IBaseCrudViewModel, new()
    where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
{
    Task<List<MaterialModifyViewModel>> GetAllMaterials();
    Task<long> SaveMaterial(MaterialModifyViewModel viewModel);
    Task<long> UpdateMaterial(MaterialModifyViewModel viewModel);
    Task<MaterialModifyViewModel> GetMaterialById(long id);
    Task<long> DeleteMaterial(long id);
}
