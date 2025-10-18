namespace Repositories.VersionService
{
    public interface IVersionService
    {
        string GetVersionNumber();
        bool GetIsUpdateForcible();
        string GetLatestApiVersion();
    }
}