namespace ViewModels.DataTable
{
    public enum DatatableHrefType
    {
        Modal,
        Link,
        Ajax
    }
    public class DisableModel
    {
        public DisableModel()
        {

        }

        public DisableModel(string property, string value)
        {
            Property = property;
            Value = value;
        }

        public string Property { get; set; }
        public string Value { get; set; }
    }
    public class DataTableActionViewModel
    {
        public string Action { get; set; }
        public string Tooltip { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; } = "";
        public string Href { get; set; }
        public string ReturnUrl { get; set; }
        public DisableModel DisableBasedOn { get; set; }
        public string HideBasedOn { get; set; }

        public List<string> Attr { get; set; }

        public string Class { get; set; }
        public bool ShowIcon { get; set; } = true;

        public DatatableHrefType DatatableHrefType { get; set; } = DatatableHrefType.Modal;

    }
}
