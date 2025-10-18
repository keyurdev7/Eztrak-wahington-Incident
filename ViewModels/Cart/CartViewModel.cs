using System;
namespace ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> InventoryItems { get; set; } = new();
        public List<CartItem> EquipmentItems { get; set; } = new();

        public int TotalItems => InventoryItems.Count + EquipmentItems.Count;

    }
}

