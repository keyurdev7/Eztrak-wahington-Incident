using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class InventoryViewModel
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string SystemGeneratedId { get; set; }
        public string ItemNo { get; set; }
        public string? Description { get; set; }
        public double MinimumQuantity { get; set; }
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public long ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        public long UOMId { get; set; }
        public string? UOMName { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public double TotalPrice { get; set; }
        public long MUTCId { get; set; }
        public string? MUTCCode { get; set; }
        public string? MUTCImageUrl { get; set; }
        public string? MUTCDescription { get; set; }
    }
}
