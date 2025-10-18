using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Helpers.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pagination;
using Repositories.Common.Example.Interface;
using Repositories.Common.Role.Interface;
using Repositories.Shared.UserInfoServices.Interface;
using Select2;
using Select2.Model;
using ViewModels;
using ViewModels.AssignedPermissions;
using ViewModels.Authentication;
using ViewModels.DataTable;
using ViewModels.Role;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    [Authorize]
    public class RoleController : CrudBaseController<RoleUpdateViewModel, RoleUpdateViewModel, RoleDetailViewModel, RoleDetailViewModel, RoleBriefViewModel, RoleSearchViewModel>
    {
        private readonly IRoleService<RoleUpdateViewModel, RoleUpdateViewModel, RoleDetailViewModel> _service;
        private readonly ILogger<RoleController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        private readonly IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> _assignedPermissionService;
        private readonly IFileHelper _fileHelper;

        public RoleController(IRoleService<RoleUpdateViewModel, RoleUpdateViewModel, RoleDetailViewModel> service, ILogger<RoleController> logger, IMapper mapper, IUserInfoService userInfoService, IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> assignedPermissionService, IFileHelper fileHelper) : base(service, logger, mapper, "Role", "Roles")
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
            _userInfoService = userInfoService;
            _assignedPermissionService = assignedPermissionService;
            _fileHelper = fileHelper;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                new DataTableActionViewModel() {Action="AssignRolePermission",Title="Permission",Href=$"/Role/AssignRolePermission/{{Id}}"},
                new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/Role/Detail/{{Id}}"},
                new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Role/Update/{{Id}}"},
                new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Role/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link}
            };
        }


        public async Task<IActionResult> AssignRolePermission(long id)
        {
            var response = new AssignPermissionDataViewModel();
            response.Role = await _assignedPermissionService.GetRole(id);
            response.EntityType = PermissionEntityType.Role;
            return View("~/Views/AssignedPermission/AssignPermission.cshtml", response);
        }

        public async Task<JsonResult> GetRoles(string prefix, int pageSize, int pageNumber, string customParams)
        {
            try
            {
                var svm = new UserRolesSearchVM();
                if (customParams != null)
                    svm = JsonConvert.DeserializeObject<UserRolesSearchVM>(customParams);

                svm.PerPage = pageSize;
                svm.CalculateTotal = true;
                svm.CurrentPage = pageNumber;
                svm.Search = new DataTableSearchViewModel() { value = prefix };
                var response = await _service.GetRoles<RoleBriefViewModel>(svm);
                PaginatedResultModel<RoleBriefViewModel> items = new();
                if (response.Status == System.Net.HttpStatusCode.OK)
                {
                    var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<RoleBriefViewModel>>;
                    items = parsedResponse?.ReturnModel ?? new();
                }
                var select2List = GetRoleSelect2List(items);
                return Json(new Select2Repository().GetSelect2PagedResult(pageSize, pageNumber, select2List, items));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Role Select2 method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

        public virtual List<Select2OptionModel<ISelect2BaseVM>> GetRoleSelect2List(PaginatedResultModel<RoleBriefViewModel> paginatedResult)
        {
            List<Select2OptionModel<ISelect2BaseVM>> response = new List<Select2OptionModel<ISelect2BaseVM>>();
            foreach (var item in paginatedResult.Items)
            {
                response.Add(new Select2OptionModel<ISelect2BaseVM>
                {
                    id = item.Id.ToString(),
                    text = item.Select2Text ?? "",
                    additionalAttributesModel = item
                });
            }

            return response.OrderBy(m => m.id).ToList();
        }

    }
}

