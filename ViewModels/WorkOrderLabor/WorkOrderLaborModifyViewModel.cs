using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using Helpers.File;
using ViewModels;
using Models;
using ViewModels.Manager;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace ViewModels
{
    public class WorkOrderLaborModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "DU")]
        [Range(1, double.MaxValue, ErrorMessage = "The field DU must be greater than zero.")]
        [Required]
        public double DU { get; set; }
        [Display(Name = "MN")]
        [Range(1, double.MaxValue, ErrorMessage = "The field MN must be greater than zero.")]
        [Required]
        public double? MN { get; set; }

        private double? _rate;
        public double? Rate
        {
            get
            {
                var result = Type == OverrideTypeCatalog.OT && (_rate == null || _rate < 1) ? Craft?.OTRate : (Type == OverrideTypeCatalog.ST ? Craft?.STRate : Craft?.DTRate);

                return result;
            }
            set => _rate = value;
        }
        public double Estimate { get { 
                var res =  DU * MN * Rate ?? 0;
                return res;
            } }
        public string RateFormatted { get => string.Format("{0:C}", Rate); }
        public string EstimateFormatted { get => string.Format("{0:C}", Estimate); }

        public double LaborRate { get; set; }
        public double LaborEstimate { get; set; }
        public OverrideTypeCatalog? Type { get; set; }
        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        public CraftSkillBriefViewModel Craft { get; set; } = new();
    }
}
