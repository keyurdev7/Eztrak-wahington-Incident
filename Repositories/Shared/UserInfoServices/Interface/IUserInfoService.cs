namespace Repositories.Shared.UserInfoServices.Interface
{
    public interface IUserInfoService
    {
        string LoggedInUserId();
        string LoggedInUserRole();
        string LoggedInEmployeeId();
        long LoggedInSupplierId();
        string LoggedInWebUserRole();
        string LoggedInUserDesignation();
        List<string> LoggedInUserRoles();
        string LoggedInUserImageUrl();
        string LoggedInUserEmail();
        string LoggedInUserFullName();
        List<long> LoggedInUserRoleIds();
    }
}
