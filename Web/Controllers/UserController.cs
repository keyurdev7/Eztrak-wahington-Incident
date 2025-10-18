using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Repositories.Common.Example.Interface;
using Repositories.Common.Users.Interface;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.AssignedPermissions;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Shared;
using ViewModels.Users;
using Web.Controllers.Shared;

namespace Web.Controllers
{

    public class UserController : UserBaseController<UserUpdateViewModel, UserUpdateViewModel, UserDetailViewModel, UserDetailViewModel, UserBriefViewModel, UserSearchViewModel>
    {
        private readonly IUserService<UserUpdateViewModel, UserUpdateViewModel, UserDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        private readonly IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> _assignedPermissionService;

        public UserController(
            IUserService<UserUpdateViewModel, UserUpdateViewModel, UserDetailViewModel> service,
            ILogger<UserController> logger,
            IMapper mapper,
            IUserInfoService userInfoService,
            IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> assignedPermissionService,
            UserManager<ApplicationUser> userManager) :
            base(service, logger, mapper, userManager, "User", "Users", RolesCatalog.SystemAdministrator, false)
        {
            _service = service;
            _mapper = mapper;
            _userInfoService = userInfoService;
            _assignedPermissionService = assignedPermissionService;
        }
        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true, sortingColumn = "FirstName"},
                new DataTableViewModel{title = "Email",data = "Username", orderable = true},
                new DataTableViewModel{title = "Role",data = "Role.DisplayName"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }

        protected override async Task<CrudUpdateViewModel> OverrideUpdateViewModel(CrudUpdateViewModel model)
        {
            var userUpdateVM = model.UpdateModel as UserUpdateViewModel;
            userUpdateVM.Roles = await _service.GetUserRolesForUpdation();
            return model;
        }

        protected override UserSearchViewModel SetSelect2CustomParams(string customParams)
        {
            if (customParams == null)
                return new UserSearchViewModel();
            var svm = JsonConvert.DeserializeObject<UserSearchViewModel>(customParams);
            return svm;
        }
        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                //new DataTableActionViewModel() {Action="Profile",Title="Profile",Href=$"/User/Profile/{{Id}}"},
                new DataTableActionViewModel() { Action = "ResetPassword", Title = "ResetPassword", Href = $"/User/ResetPassword/{{Id}}",HideBasedOn="CantResetPassword" },
                new DataTableActionViewModel() { Action = "ResetPinCode", Title = "ResetPassword", Href = $"/User/ResetPinCode/{{Id}}",HideBasedOn="CantResetPinCode" },
                new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/User/Update/{{Id}}"},
                // new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/User/Detail/{{Id}}"},
                new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/User/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link}
            };
        }

        public async Task<IActionResult> AssignUserPermission(long id)
        {
            var response = new AssignPermissionDataViewModel();
            response.User = await _assignedPermissionService.GetUser(id);
            response.EntityType = PermissionEntityType.User;
            return View("~/Views/AssignedPermission/AssignPermission.cshtml", response);
        }
    }
}
