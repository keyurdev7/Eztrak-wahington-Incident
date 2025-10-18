using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.Common;
using ViewModels;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _inventoryService;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> _equipmentTransactionService;

        //private readonly IDashboardService _service;

        public CartController(
            ILogger<CartController> logger
            , IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> inventoryService
            , ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService
            , IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> equipmentTransactionService
            )
        {
            _logger = logger;
            _inventoryService = inventoryService;
            _transactionService = transactionService;
            _equipmentTransactionService = equipmentTransactionService;
        }

        public async Task<ActionResult> Index()
        {
            var response = new OrderModifyViewModel();
            try
            {
                var cart = GetCartFromSession();
                var model = new OrderModifyViewModel();
                var inventoryIds = cart.InventoryItems.Select(x => x.InventoryId).ToList();
                if (inventoryIds?.Count > 0)
                {
                    var orderedInventories = await _transactionService.GetGroupedTransactionsByItemsForOrder(inventoryIds);
                    foreach (var o in orderedInventories)
                    {
                        response.OrderItems.Add(new OrderItemModifyViewModel
                        {
                            Inventory = o.Inventory,
                            OHQuantity = (long)o.Quantity,
                            Quantity = cart.InventoryItems.Where(x => x.InventoryId == o.Inventory.Id).Select(x => x.Quantity).FirstOrDefault(),
                        });
                    }
                }
                var equipmentIds = cart.EquipmentItems.Select(x => x.EquipmentId).ToList();
                if (equipmentIds?.Count > 0)
                {
                    var orderedInventories = await _equipmentTransactionService.GetGroupedTransactionsByItemsForOrder(equipmentIds);
                    foreach (var o in orderedInventories)
                    {
                        response.OrderItems.Add(new OrderItemModifyViewModel
                        {
                            Equipment = o.Equipment,
                            OHQuantity = (long)o.Quantity,
                            Quantity = cart.EquipmentItems.Where(x => x.EquipmentId == o.Equipment.Id).Select(x => x.Quantity).FirstOrDefault(),
                        });
                    }
                }

                if (equipmentIds?.Count < 1 && inventoryIds?.Count < 1)
                {
                    ModelState.AddModelError("", "Add some items in the cart first!");
                }
                return View(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong, please try again later.");
                _logger.LogError(ex, ex.Message);
                return View(response);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteInventoryFromCart(int id)
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItem = cart?.InventoryItems.Where(x => x.InventoryId == id).FirstOrDefault();
                if (cartItem != null)
                {
                    cart?.InventoryItems.Remove(cartItem);
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                    if (cart?.InventoryItems.Count == 0)
                        return Json(true);
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(false);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEquipmentFromCart(int id)
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItem = cart?.EquipmentItems.Where(x => x.EquipmentId == id).FirstOrDefault();
                if (cartItem != null)
                {
                    cart?.EquipmentItems.Remove(cartItem);
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                    if (cart?.EquipmentItems.Count == 0)
                        return Json(true);
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(false);
            }
        }

        public PartialViewResult ShowShoppingCart()
        {

            var cart = GetCartFromSession();
            return PartialView("_ShowShoppingCart", cart);
        }

        [HttpPost]
        public async Task<JsonResult> AddInventoryToCart(long id)
        {
            try
            {
                var cart = GetCartFromSession();

                var existinginventoryinfo = cart.InventoryItems.Where(x => x.InventoryId == id).FirstOrDefault();
                if (existinginventoryinfo != null)
                {
                    ++existinginventoryinfo.Quantity;
                }
                else
                {
                    CartItem cartitem = new CartItem()
                    {
                        InventoryId = id,
                        Quantity = 1,
                    };
                    cart.InventoryItems.Add(cartitem);
                }
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<JsonResult> AddEquipmentToCart(long id)
        {
            try
            {
                var cart = GetCartFromSession();

                var existinginventoryinfo = cart.EquipmentItems.Where(x => x.EquipmentId == id).FirstOrDefault();
                if (existinginventoryinfo != null)
                {
                    ++existinginventoryinfo.Quantity;
                }
                else
                {
                    CartItem cartitem = new CartItem()
                    {
                        EquipmentId = id,
                        Quantity = 1,
                    };
                    cart.EquipmentItems.Add(cartitem);
                }
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Json(true);
        }

        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            CartViewModel cart = new();
            if (cartJson != null)
            {
                cart = JsonConvert.DeserializeObject<CartViewModel>(cartJson) ?? new();
                // Now you can use the 'cart' object
            }
            return cart;
        }

    }
}