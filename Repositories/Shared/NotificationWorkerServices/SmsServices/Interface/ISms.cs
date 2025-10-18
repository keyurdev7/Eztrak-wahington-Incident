using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISms
    {
        Task<bool> SendSms(string toNumber, string MessageText);
    }
}
