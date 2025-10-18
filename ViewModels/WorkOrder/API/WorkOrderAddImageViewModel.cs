using Models.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class WorkOrderAddImageViewModel : IIdentitifier
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}
