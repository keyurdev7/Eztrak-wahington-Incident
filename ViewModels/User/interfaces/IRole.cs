using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Role;

namespace ViewModels.User.interfaces
{
    public interface IRole
    {
        RoleBriefViewModel? Role { get; set; }
    }
}
