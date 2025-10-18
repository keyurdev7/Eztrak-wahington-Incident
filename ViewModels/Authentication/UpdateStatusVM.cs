using System;
namespace ViewModels.Authentication
{
    public class UpdateStatusVM
    {
        public UpdateStatusVM()
        {
        }
        public string LatestVersion { get; set; }
        public bool IsForcible { get; set; }
    }
}

