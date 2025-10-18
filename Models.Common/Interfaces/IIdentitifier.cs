namespace Models.Common.Interfaces
{
    public interface IIdentitifier
    {
        long Id { get; set; }
    }
    public interface INullableIdentitifier
    {
        long? Id { get; set; }
    }
}
