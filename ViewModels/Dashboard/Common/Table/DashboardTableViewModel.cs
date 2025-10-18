namespace ViewModels.Dashboard.Common.Table
{
    public class DashboardTableViewModel
    {
        public DashboardTableViewModel()
        {

        }
        public DashboardTableViewModel(string id, string title, string url)
        {
            Id = id;
            Title = title;
            Url = url;
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public class DashboardTableDataViewModel<T>
    {
        public List<T> TableData { get; set; } = new List<T>();
    }

}
