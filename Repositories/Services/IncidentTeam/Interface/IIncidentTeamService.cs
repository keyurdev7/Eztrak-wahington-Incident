using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

public interface IIncidentTeamService<CreateViewModel, UpdateViewModel, DetailViewModel>
    : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
    where DetailViewModel : class, IBaseCrudViewModel, new()
    where CreateViewModel : class, IBaseCrudViewModel, new()
    where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
{
    Task<List<IncidentTeamModifyViewModel>> GetAllIncidentTeams();
    Task<long> SaveIncidentTeam(IncidentTeamModifyViewModel viewModel);
    Task<long> UpdateIncidentTeam(IncidentTeamModifyViewModel viewModel);
    Task<IncidentTeamModifyViewModel> GetIncidentTeamById(long id);
    Task<long> DeleteIncidentTeam(long id);
}
