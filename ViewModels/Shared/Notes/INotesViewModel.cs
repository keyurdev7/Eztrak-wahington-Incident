using Helpers.Datetime;
using Helpers.File;

namespace ViewModels.Shared.Notes
{
    public interface INotesViewModel : IFileModel
    {
        string Description { get; set; }
        string? FileUrl { get; set; }
        DateTime CreatedOn { get; set; }
        string CreatedBy { get; set; }
        string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDateInPST();
            }
        }
    }
}
