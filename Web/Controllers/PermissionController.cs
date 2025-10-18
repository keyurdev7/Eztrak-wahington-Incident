using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common.Permission.Interface;
using Repositories.Interfaces;
using ViewModels.DataTable;
using ViewModels.Permission;
using ViewModels.Shared;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class PermissionController : CrudBaseController<PermissionUpdateViewModel, PermissionUpdateViewModel, PermissionDetailViewModel, PermissionDetailViewModel,PermissionBriefViewModel, PermissionSearchViewModel>
    {
        private readonly IPermissionService<PermissionUpdateViewModel, PermissionUpdateViewModel, PermissionDetailViewModel> _service;

        public PermissionController(IPermissionService<PermissionUpdateViewModel, PermissionUpdateViewModel, PermissionDetailViewModel> service, ILogger<PermissionController> logger, IMapper mapper) : base(service, logger, mapper, "Permission", "Permission")
        {
            _service = service;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Screen",data = "Screen"},
                new DataTableViewModel{title = "Action",data = null,className="text-right exclude-form-export"}

            };
        }

        public IActionResult ImportExcelSheet()
        {
            var model = new ExcelFileVM();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ImportExcelSheet(ExcelFileVM model)
        {
            await _service.ProcessExcelPermissionsData(model);
            return RedirectToAction("Index");
        }
    }
}

