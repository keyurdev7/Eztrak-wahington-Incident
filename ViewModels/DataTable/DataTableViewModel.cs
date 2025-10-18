namespace ViewModels.DataTable
{
    public class DataTableViewModel
    {
        public string title { get; set; }
        public string data { get; set; }
        public string format { get; set; }
        public string formatValue { get; set; }
        public bool orderable { get; set; }
        private string _sortingColumn;
        public string sortingColumn { get => string.IsNullOrEmpty(_sortingColumn) ? data : _sortingColumn; set => _sortingColumn = value; }
        public string className { get; set; }
        public bool isEditable { get; set; }
        public EditableCellDetails editableColumnDetail { get; set; } = new();
        public List<DropDownOptions> options { get; set; } = new();
        public string filterId { get; set; }

        public bool hasFilter { get; set; }
    }

    public class DropDownOptions
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class EditableCellDetails
    {
        public EditableCellDetails() { }
        public EditableCellDetails(string entityName, string entityProperty, string cellName, string fieldName, string hiddenFormId, bool renderTooltip = false, string dataIdentifier = "Id", List<(string, string)>? additionalFields = null)
        {

            this.entityName = entityName;
            this.entityProperty = entityProperty;
            this.cellName = cellName;
            this.fieldName = fieldName;
            this.hiddenFormId = hiddenFormId;
            this.renderTooltip = renderTooltip;
            this.dataIdentifier = dataIdentifier;
            this.additionalFields = additionalFields ?? new List<(string, string)> { };

        }


        public string entityName { get; set; } = "";
        public string entityProperty { get; set; } = "";
        public string cellName { get; set; } = "";
        public string fieldName { get; }
        public string hiddenFormId { get; set; } = "";
        public bool renderTooltip { get; set; }
        public string dataIdentifier { get; }
        public List<(string, string)> additionalFields { get; set; }
    }
}
