namespace ViewModels.Dashboard.interfaces
{
    public interface ITimePeriodSearch
    {
        DateTime? FromDate { get; set; }
        DateTime? ToDate { get; set; }
    }
}