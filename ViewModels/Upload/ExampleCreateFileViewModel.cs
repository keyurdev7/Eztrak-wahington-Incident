using System;
using Microsoft.AspNetCore.Http;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels.Example
{
    public class ExampleCreateFileViewModel : BaseCreateVM
    {
        public ExampleCreateFileViewModel()
        {
        }
        public IFormFile File { get; set; }
    }
}

