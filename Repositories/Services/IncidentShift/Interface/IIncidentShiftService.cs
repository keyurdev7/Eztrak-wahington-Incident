using Models.Common.Interfaces;
using Repositories.Interfaces;
using System;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IIncidentShiftService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<IncidentShiftModifyViewModel>> GetAllIncidentShifts();
        Task<long> SaveIncidentShift(IncidentShiftModifyViewModel viewModel);
        Task<long> UpdateIncidentShift(IncidentShiftModifyViewModel viewModel);
        Task<IncidentShiftModifyViewModel> GetIncidentShiftById(long Id);
        Task<long> DeleteIncidentShift(long id);
    }
}
