using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pagination;
using ViewModels.Authentication;
using ViewModels.User.interfaces;

namespace Repositories.Services.AuthenticationService.Interface
{
    public interface IIdentityService
    {
        Task<IRepositoryResponse> CreateUser(
           IUserCreateViewModel model,
           ModelStateDictionary ModelState,
           string optionalUsernamePrefix = ""
           );
        Task<IRepositoryResponse> UpdateUser(
            IUserUpdateViewModel model,
            ModelStateDictionary ModelState
            );
        Task<IRepositoryResponse> SignUp(
            SignUpVM model,
            ModelStateDictionary ModelState
            );
        Task<IRepositoryResponse> Delete(long id);
        Task<IRepositoryResponse> Approve(long id);

        Task<UserVM> GetById(long id);

        Task<IRepositoryResponse> RecoverPassword(ResetPasswordVM model, ModelStateDictionary ModelState);

        Task<IRepositoryResponse> ChangePassword(ChangePasswordVM model, ModelStateDictionary ModelState);

        Task<IRepositoryResponse> UserDetail();

        Task<IRepositoryResponse> GetAll<T>(BaseSearchModel search);
        Task<bool> IsUsernameUnique(long id, string username);
        Task<bool> IsEmailUnique(long id, string email);
    }
}