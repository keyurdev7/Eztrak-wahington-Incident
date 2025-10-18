using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class InventoriesController : Controller
    {
        public IActionResult Index(string activeTab)
        {
            var tab = new TabViewModel()
            {
                Id = "inventory-tab",
                ActiveTab = activeTab,
                ContentId = "inventory-tab-content-Id",
                ContainerClassName = "inventory_header",
                CustomNavHtml = @"<ul class=""temp-nav-tabs-action-buttons""> 
                       <li>
                        <button type=""button"" onclick=""loadCreateModalPanel('/Equipment/Create')"" data-bs-target=""#newSupModal"">
                            <svg width=""17"" height=""16"" viewBox=""0 0 17 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
							<path d=""M8.225 12.3498H8.925V8.3498H12.925V7.6498H8.925V3.6498H8.225V7.6498H4.225V8.3498H8.225V12.3498ZM2.375 15.6998C1.94167 15.6998 1.58333 15.5581 1.3 15.2748C1.01667 14.9915 0.875 14.6331 0.875 14.1998V1.7998C0.875 1.36647 1.01667 1.00814 1.3 0.724805C1.58333 0.441471 1.94167 0.299805 2.375 0.299805H14.775C15.2083 0.299805 15.5667 0.441471 15.85 0.724805C16.1333 1.00814 16.275 1.36647 16.275 1.7998V14.1998C16.275 14.6331 16.1333 14.9915 15.85 15.2748C15.5667 15.5581 15.2083 15.6998 14.775 15.6998H2.375ZM2.375 14.9998H14.775C14.975 14.9998 15.1583 14.9165 15.325 14.7498C15.4917 14.5831 15.575 14.3998 15.575 14.1998V1.7998C15.575 1.5998 15.4917 1.41647 15.325 1.2498C15.1583 1.08314 14.975 0.999805 14.775 0.999805H2.375C2.175 0.999805 1.99167 1.08314 1.825 1.2498C1.65833 1.41647 1.575 1.5998 1.575 1.7998V14.1998C1.575 14.3998 1.65833 14.5831 1.825 14.7498C1.99167 14.9165 2.175 14.9998 2.375 14.9998Z"" fill=""#F54C00""/>
						</svg>
                            Add New
                        </button>
                        </li>
                    <button type=""button"" onclick=""loadSublinesView('/Equipment/_AddSublines')"" data-bs-target=""#newSupModal"">
                        <svg width=""17"" height=""16"" viewBox=""0 0 17 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
							<path d=""M8.225 12.3498H8.925V8.3498H12.925V7.6498H8.925V3.6498H8.225V7.6498H4.225V8.3498H8.225V12.3498ZM2.375 15.6998C1.94167 15.6998 1.58333 15.5581 1.3 15.2748C1.01667 14.9915 0.875 14.6331 0.875 14.1998V1.7998C0.875 1.36647 1.01667 1.00814 1.3 0.724805C1.58333 0.441471 1.94167 0.299805 2.375 0.299805H14.775C15.2083 0.299805 15.5667 0.441471 15.85 0.724805C16.1333 1.00814 16.275 1.36647 16.275 1.7998V14.1998C16.275 14.6331 16.1333 14.9915 15.85 15.2748C15.5667 15.5581 15.2083 15.6998 14.775 15.6998H2.375ZM2.375 14.9998H14.775C14.975 14.9998 15.1583 14.9165 15.325 14.7498C15.4917 14.5831 15.575 14.3998 15.575 14.1998V1.7998C15.575 1.5998 15.4917 1.41647 15.325 1.2498C15.1583 1.08314 14.975 0.999805 14.775 0.999805H2.375C2.175 0.999805 1.99167 1.08314 1.825 1.2498C1.65833 1.41647 1.575 1.5998 1.575 1.7998V14.1998C1.575 14.3998 1.65833 14.5831 1.825 14.7498C1.99167 14.9165 2.175 14.9998 2.375 14.9998Z"" fill=""#F54C00""/>
						</svg>
                        Add Shipment
                    </button>
                </li>
                
            </ul>",
                //CustomTitleHtml = @"<div class=""total"">
                //         Total Inventory: <span id=""inventory-total-price""></span>
                //    </div>",
                Title = "Inventory",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="inventory",
                        Name="Material",
                        Url="/Inventory/Index"

                    },
                    new TabItemViewModel()
                    {
                        Id="equipment",
                        Name="Equipment",
                        Url="/Equipment/Index"

                    },
                    new TabItemViewModel()
                    {
                        Id="orders",
                        Name="Orders",
                        Url="/Order/Index"

                    },
                    new TabItemViewModel()
                    {
                        Id="execute",
                        Name="Execute",
                        Url="/Execute/Index"

                    }
                }
            };
            return View(tab);
        }
    }
}
