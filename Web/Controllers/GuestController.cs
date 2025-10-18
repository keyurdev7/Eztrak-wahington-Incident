using AutoMapper;
using ViewModels.DataTable;
using ViewModels.Guest;
using Enums;
using Microsoft.AspNetCore.Identity;
using Models;
using Repositories.Common.GuestService.Interface;
using Web.Controllers.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class GuestController : UserBaseController<GuestUpdateViewModel, GuestUpdateViewModel, GuestDetailViewModel, GuestDetailViewModel, GuestSelect2ViewModel, GuestSearchViewModel>
    {
        private readonly IGuestService<GuestUpdateViewModel, GuestUpdateViewModel, GuestDetailViewModel> _service;
        private readonly bool _hasAdditionalInfo;

        public GuestController(
                   IGuestService<GuestUpdateViewModel, GuestUpdateViewModel, GuestDetailViewModel> service,
                   ILogger<UserController> logger,
                   IMapper mapper,
                   UserManager<ApplicationUser> userManager,
                   bool hasAdditionalInfo = false) :
                   base(service, logger, mapper, userManager, "Guest", "Guests", RolesCatalog.Guest, hasAdditionalInfo)
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
                   // new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/Guest/Detail/{{Id}}"},
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Guest/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Guest/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
                };
            if (User.IsInRole("SystemAdministrator"))
            {
                result.ActionsList.Add(new DataTableActionViewModel() { Action = "ResetPassword", Title = "ResetPassword", Href = $"/Guest/ResetPassword/{{Id}}" });
            }
        }
    }

}

