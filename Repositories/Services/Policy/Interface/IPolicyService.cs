using System;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IPolicyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<PolicyModifyViewModel>> GetAllPolicies();
        Task<long> SavePolicy(PolicyModifyViewModel viewModel);
        Task<long> UpdatePolicy(PolicyModifyViewModel viewModel);
        Task<PolicyModifyViewModel> GetPolicyById(long id);
        Task<long> DeletePolicy(long id);
       // Task<long> AddPolicyStep(long policyId, string step);
        Task<long> AddPolicySteps(long policyId, IEnumerable<string> steps);

    }
}
