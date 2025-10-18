using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IEmail
    {
        Task<bool> SendEmail(string to, string from, string subject, string htmlMessage);
    }
}
