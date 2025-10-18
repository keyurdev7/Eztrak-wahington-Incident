using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class RelationshipBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public RelationshipBriefViewModel() : base(true, "The Relationship field is required.")
        {

        }

        public RelationshipBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }

        [DisplayName("Relationship")]
        public string? Name { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }
}