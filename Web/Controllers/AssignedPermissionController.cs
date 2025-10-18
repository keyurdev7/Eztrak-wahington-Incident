using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common.Example.Interface;
using ViewModels.DataTable;
using ViewModels.AssignedPermissions;
using Enums;
using ViewModels.Permission;
using Centangle.Common.ResponseHelpers.Models;
using ViewModels;
using Web.Controllers.Shared;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace Web.Controllers
{
    //[AllowAnonymous]
    public class AssignedPermissionController : CrudBaseController<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel, AssignedPermissionDetailViewModel,BaseSelect2VM, AssignedPermissionSearchViewModel>
    {
        private readonly IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> _service;

        public AssignedPermissionController(IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> service, ILogger<AssignedPermissionController> logger, IMapper mapper) : base(service, logger, mapper, "AssignedPermission", "AssignedPermission")
        {
            _service = service;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="text-right exclude-form-export"}

            };
        }
        [Authorize(Policy = "CreateProject")]
        public async Task<IActionResult> AssignPermission()
        {
            var response = new AssignPermissionDataViewModel();
            response.IsGeneralPermissionsScreen = true;
            return View(response);
        }

        [Authorize(Policy = "CreateProject")]
        public async Task<IActionResult> GetPermissions(long entityId, PermissionEntityType entityType)
        {
            var permissions = await _service.GetAssignedPermissions(new AssignedPermissionSearchViewModel { EntityId = entityId, EntityType = entityType });
            var assignedPermissionUpdateViewModels = new List<AssignedPermissionUpdateViewModel>
            {
                new AssignedPermissionUpdateViewModel {Id=1,    Permission=new PermissionBriefViewModel{ Id = 1,Name = "Add Product", Screen = "Settings/Product" }, Priority = 1 },
                new AssignedPermissionUpdateViewModel {Id=2,    Permission=new PermissionBriefViewModel{ Id = 2,Name = "Delete Product", Screen = "Settings/Product" }, Priority = 4 },
                new AssignedPermissionUpdateViewModel {Id=3,    Permission=new PermissionBriefViewModel{ Id = 3,Name = "Edit Product", Screen = "Settings/Product" }, Priority = 2 },
                new AssignedPermissionUpdateViewModel {Id = 4,  Permission=new PermissionBriefViewModel{ Id = 4,Name = "View Product", Screen = "Settings/Product" }, Priority = 3 },
                new AssignedPermissionUpdateViewModel {Id=5,    Permission=new PermissionBriefViewModel{ Id = 5,Name = "Add Product Variant", Screen = "Settings/Product/Variant" }, Priority = 1 },
                new AssignedPermissionUpdateViewModel {Id=6,    Permission=new PermissionBriefViewModel{ Id = 6,Name = "Delete Product Variant", Screen = "Settings/Product/Variant" }, Priority = 4 },
                new AssignedPermissionUpdateViewModel {Id=7,    Permission=new PermissionBriefViewModel{ Id = 7,Name = "Edit Product Variant", Screen = "Settings/Product/Variant" }, Priority = 2 },
                new AssignedPermissionUpdateViewModel {Id=8,    Permission=new PermissionBriefViewModel{ Id = 8,Name = "View Product Variant", Screen = "Settings/Product/Variant" }, Priority = 3 },
                new AssignedPermissionUpdateViewModel {Id=9,    Permission=new PermissionBriefViewModel{ Id = 9,Name = "Add Tax", Screen = "Settings/Tax" }, Priority = 1 },
                new AssignedPermissionUpdateViewModel {Id=10,   Permission=new PermissionBriefViewModel{ Id = 10,Name = "Delete Tax", Screen = "Settings/Tax" }, Priority = 4 },
                new AssignedPermissionUpdateViewModel {Id=11,   Permission=new PermissionBriefViewModel{ Id = 11,Name = "Edit Tax", Screen = "Settings/Tax" }, Priority = 2 },
                new AssignedPermissionUpdateViewModel {Id=12,   Permission=new PermissionBriefViewModel{ Id = 12,Name = "View Tax", Screen = "Settings/Tax" }, Priority = 3 },
                new AssignedPermissionUpdateViewModel {Id=13,   Permission=new PermissionBriefViewModel{ Id = 13,Name = "Add Sale", Screen = "Transactions/Sale" }, Priority = 1 },
                new AssignedPermissionUpdateViewModel {Id=14,   Permission=new PermissionBriefViewModel{ Id = 14,Name = "View Sale", Screen = "Transactions/Sale" }, Priority = 2 },
                new AssignedPermissionUpdateViewModel {Id=15,   Permission=new PermissionBriefViewModel{ Id = 15,Name = "Update Sale", Screen = "Transactions/Sale" }, Priority = 3 },
                new AssignedPermissionUpdateViewModel {Id=16,   Permission=new PermissionBriefViewModel{ Id = 16,Name = "Delete Sale", Screen = "Transactions/Sale" }, Priority = 4 }
            };
            var parsedResponse = permissions as RepositoryResponseWithModel<AssignPermissionDataViewModel>;
            return View(parsedResponse?.ReturnModel?.Permissions ?? new List<AssignedPermissionUpdateViewModel>());
        }

        [HttpPost]
        public async Task<JsonResult> SetAssignedPermissions([FromBody] AssignPermissionDataViewModel model)
        {
            var response = await _service.AssignPermissions(model);
            return Json(response);
        }
        [Authorize(Policy = "CreateProject")]
        public async Task<ActionResult> GetPolicies()
        {
            var search = new AssignedPermissionSearchViewModel { EntityId = 1, EntityType = Enums.PermissionEntityType.Role, Status = Enums.PermissionStatus.Allowed };
            var policies = await _service.GetAssignedPermissions(search);
            return View(policies);
        }


    }
}

