using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;

namespace ViewModels
{
    public class AssetRePinViewModel : IIdentitifier
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
