using Enums;

namespace Models.Common.Interfaces
{
    public interface IIsActive
    {
        ActiveStatus ActiveStatus { get; set; }
    }
}
