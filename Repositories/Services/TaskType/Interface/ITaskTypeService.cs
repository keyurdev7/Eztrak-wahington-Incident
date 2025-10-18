using System;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface ITaskTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<TaskMaterialViewModel>> GetTaskMaterials(long id);
        Task<List<TaskEquipmentViewModel>> GetTaskEquipments(long id);
        Task<List<TaskLaborViewModel>> GetTaskLabors(long id);
        Task<List<TaskWorkStepViewModel>> GetTaskSteps(long id);
    }
}

