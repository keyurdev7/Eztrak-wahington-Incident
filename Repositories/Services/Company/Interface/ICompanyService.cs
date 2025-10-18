using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface ICompanyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<CompanyModifyViewModel>> GetAllCompanys();
        Task<long> SaveCompany(CompanyModifyViewModel viewModel);
        Task<long> UpdateCompany(CompanyModifyViewModel viewModel);
        Task<CompanyModifyViewModel> GetCompanyById(long Id);
        Task<long> DeleteCompany(long id);
    }
}