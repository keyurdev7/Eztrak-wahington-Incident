using Helpers.File;
using ViewModels.Users;

namespace ViewModels.User.interfaces
{
    public interface IUserUpdateViewModel : IEmail, IFileModel, IRole
    {
        bool HasAdditionInfo { get; set; }
        long UserId { get; set; }
    }
}
