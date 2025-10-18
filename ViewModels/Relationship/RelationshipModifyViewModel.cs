using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;

namespace ViewModels
{
    public class RelationshipModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {

        [Display(Name = "Source", Prompt = "Source")]
        public string Name { get; set; }
    }
}
