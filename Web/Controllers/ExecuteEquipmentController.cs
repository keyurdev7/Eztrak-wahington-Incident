using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using Repositories.Common.Users;
using ViewModels;

namespace Web.Controllers
{
    public class ExecuteEquipmentController : Controller
    {
        private readonly IExecuteEquipmentService _executeEquipmentService;
        private readonly IMapper _mapper;

        public ExecuteEquipmentController(IExecuteEquipmentService executeEquipmentService, IMapper mapper)
        {
            _executeEquipmentService = executeEquipmentService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ValidateUserPin(int pinCode)
        {
            if (pinCode == 5555)
            {
                return Json(true);

            }
            return Json(false);
        }


        public ActionResult ReturnEquipment()
        {
            return View();
        }


        public async Task<ActionResult> GetEquipmentToReturn(string itemNo)
        {
            var items = await _executeEquipmentService.GetGroupedOrderTransactionsByEquipments(itemNo);
            var model = new ReturnEquipmentViewModel();
            model.Transactions = _mapper.Map<List<ReturnEquipmentListViewModel>>(items);

            return PartialView("_EquipmentsToReturn", model);
        }


        public async Task<ActionResult> ReturnInventoryEquipment(ReturnEquipmentViewModel model)
        {
            var response = await _executeEquipmentService.ReturnEquipments(model);
            return Json(response);
        }

    }
}
