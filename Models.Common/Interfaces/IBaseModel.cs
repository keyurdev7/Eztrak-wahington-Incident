namespace Models.Common.Interfaces
{
    public interface IBaseModel : IIsDeleted, IIsActive
    {
        long Id { get; set; }
        DateTime CreatedOn { get; set; }
        long CreatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
        long UpdatedBy { get; set; }
    }
}
