using AutoMapper;
using ViewModels.DataTable;
using ViewModels.Administrator;
using Enums;
using Microsoft.AspNetCore.Identity;
using Models;
using Repositories.Common.AdministratorService.Interface;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AdministratorController : UserBaseController<AdministratorUpdateViewModel, AdministratorUpdateViewModel, AdministratorDetailViewModel, AdministratorDetailViewModel, AdministratorSelect2ViewModel, AdministratorSearchViewModel>
    {
        private readonly IAdministratorService<AdministratorUpdateViewModel, AdministratorUpdateViewModel, AdministratorDetailViewModel> _service;
        private readonly bool _hasAdditionalInfo;

        public AdministratorController(
                   IAdministratorService<AdministratorUpdateViewModel, AdministratorUpdateViewModel, AdministratorDetailViewModel> service,
                   ILogger<UserController> logger,
                   IMapper mapper,
                   UserManager<ApplicationUser> userManager,
                   bool hasAdditionalInfo = false) :
                   base(service, logger, mapper, userManager, "Administrator", "Administrators", RolesCatalog.SystemAdministrator, hasAdditionalInfo)
        {
            _service = service;
            _hasAdditionalInfo = hasAdditionalInfo;
        }


        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "First Name",data = "FirstName"},
                new DataTableViewModel{title = "Last Name",data = "LastName"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                    //new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/Administrator/Detail/{{Id}}"},
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Administrator/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Administrator/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
                };
            if (User.IsInRole("SystemAdministrator"))
            {
                result.ActionsList.Add(new DataTableActionViewModel() { Action = "ResetPassword", Title = "ResetPassword", Href = $"/Administrator/ResetPassword/{{Id}}" });
            }
        }
    }
}

