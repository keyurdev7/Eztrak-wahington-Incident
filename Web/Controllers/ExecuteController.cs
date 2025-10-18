using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using Repositories.Common.Users;
using ViewModels;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class ExecuteController : Controller
    {
        private readonly IExecuteService _executeService;
        private readonly IMapper _mapper;

        public ExecuteController(IExecuteService executeService, IMapper mapper)
        {
            _executeService = executeService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult ReStageInventory()
        {
            return View();
        }
        public ActionResult ReturnInventory()
        {
            return View();
        }

        public ActionResult RemoveInventory()
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

        public async Task<ActionResult> GetItemsToRestage(string lotNo)
        {
            var items = await _executeService.GetGroupedTransactionsByItems(lotNo);
            var model = new ReStageViewModel();
            model.Transactions = _mapper.Map<List<ReStageListViewModel>>(items);

            return PartialView("_ItemsToReStage", model);
        }


        public async Task<ActionResult> ReStageItems(ReStageViewModel model)
        {
            var response = await _executeService.ReStageItems(model);
            return Json(response);
        }

        public async Task<ActionResult> GetItemsToRemove(string lotNo)
        {
            var items = await _executeService.GetGroupedTransactionsByItems(lotNo);
            var model = new RemoveInventoryItemsViewModel();
            model.Transactions = _mapper.Map<List<RemoveInventoryItemsListViewModel>>(items);

            return PartialView("_ItemsToRemove", model);
        }

        public async Task<ActionResult> RemoveInventoryItems(RemoveInventoryItemsViewModel model)
        {
            var response = await _executeService.RemoveInventoryItems(model);
            return Json(true);
        }

        public async Task<ActionResult> GetItemsToReturn(string lotNo)
        {
            var items = await _executeService.GetGroupedOrderTransactionsByItems(lotNo);
            var model = new ReturnInventoryItemsViewModel();
            model.Transactions = _mapper.Map<List<ReturnInventoryItemsListViewModel>>(items);

            return PartialView("_ItemsToReturn", model);
        }


        public async Task<ActionResult> ReturnInventoryItems(ReturnInventoryItemsViewModel model)
        {
            var response = await _executeService.ReturnInventoryItems(model);
            return Json(response);
        }
 
    }
}
