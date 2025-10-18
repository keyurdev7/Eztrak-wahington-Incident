using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Azure;
using Models;
using Repositories.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using ViewModels;
using ViewModels.Shared;

namespace Web.Controllers
{
    public class SeedController : Controller
    {
        private readonly IManufacturerService<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel> _manufacturerService;
        private readonly IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> _conditionService;
        private readonly ICraftSkillService<CraftSkillModifyViewModel, CraftSkillModifyViewModel, CraftSkillDetailViewModel> _craftSkillService;
        private readonly ICategoryService<CategoryModifyViewModel, CategoryModifyViewModel, CategoryDetailViewModel> _categoryService;
        private readonly ISourceService<SourceModifyViewModel, SourceModifyViewModel, SourceDetailViewModel> _sourceService;
        private readonly ISupplierService<SupplierModifyViewModel, SupplierModifyViewModel, SupplierDetailViewModel> _suppplierService;
        private readonly IUOMService<UOMModifyViewModel, UOMModifyViewModel, UOMDetailViewModel> _uomService;
        private readonly IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> _mutcdService;
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _assetService;
        private readonly IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> _assetTypeService;
        private readonly IAssetTypeLevel1Service<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel> _assetTypeLevel1Service;
        private readonly IAssetTypeLevel2Service<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel> _assetTypeLevel2Service;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILocationService<LocationModifyViewModel, LocationModifyViewModel, LocationDetailViewModel> _locationService;

        public SeedController(
            IManufacturerService<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel> manufacturerService,
            IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> conditionService,
            ICraftSkillService<CraftSkillModifyViewModel, CraftSkillModifyViewModel, CraftSkillDetailViewModel> craftSkillService,
            ICategoryService<CategoryModifyViewModel, CategoryModifyViewModel, CategoryDetailViewModel> categoryService,
            ILocationService<LocationModifyViewModel, LocationModifyViewModel, LocationDetailViewModel> locationService,
            ISourceService<SourceModifyViewModel, SourceModifyViewModel, SourceDetailViewModel> sourceService,
            ISupplierService<SupplierModifyViewModel, SupplierModifyViewModel, SupplierDetailViewModel> suppplierService,
            IUOMService<UOMModifyViewModel, UOMModifyViewModel, UOMDetailViewModel> uomService,
            IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> mutcdService,
            IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> assetService,
            IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> assetTypeService,
            IAssetTypeLevel1Service<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel> assetTypeLevel1Service,
            IAssetTypeLevel2Service<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel> assetTypeLevel2Service,
            IWebHostEnvironment hostingEnvironment

            )
        {
            _manufacturerService = manufacturerService;
            _conditionService = conditionService;
            _craftSkillService = craftSkillService;
            _categoryService = categoryService;
            _sourceService = sourceService;
            _suppplierService = suppplierService;
            _uomService = uomService;
            _mutcdService = mutcdService;
            _assetService = assetService;
            _assetTypeService = assetTypeService;
            _assetTypeLevel1Service = assetTypeLevel1Service;
            _assetTypeLevel2Service = assetTypeLevel2Service;
            _hostingEnvironment = hostingEnvironment;
            _locationService = locationService;
        }
        public async Task<IActionResult> Index()
        {
            await GenerateRandomMUTCD();
            await GenerateRandomManufacturers();
            await GenerateRandomConditions();
            await GenerateRandomCraftSkills();
            await GenerateRandomCategories();
            await GenerateRandomLocation();
            await GenerateRandomSupplier();
            await GenerateRandomSource();
            await GenerateRandomUOm();
            await GenerateAssetTypesAndAssociations();
            return View();
        }
        [Route("generate/random/manufacturers")]
        public async Task<IActionResult> GenerateRandomManufacturersAction()
        {
            await GenerateRandomManufacturers();
            return Ok("Manufacturers generated successfully.");
        }

        [Route("generate/random/conditions")]
        public async Task<IActionResult> GenerateRandomConditionsAction()
        {
            await GenerateRandomConditions();
            return Ok("Conditions generated successfully.");
        }

        [Route("generate/random/craftskills")]
        public async Task<IActionResult> GenerateRandomCraftSkillsAction()
        {
            await GenerateRandomCraftSkills();
            return Ok("Craft skills generated successfully.");
        }

        [Route("generate/random/categories")]
        public async Task<IActionResult> GenerateRandomCategoriesAction()
        {
            await GenerateRandomCategories();
            return Ok("Categories generated successfully.");
        }

        [Route("generate/random/locations")]
        public async Task<IActionResult> GenerateRandomLocationAction()
        {
            await GenerateRandomLocation();
            return Ok("Locations generated successfully.");
        }

        [Route("generate/random/sources")]
        public async Task<IActionResult> GenerateRandomSourceAction()
        {
            await GenerateRandomSource();
            return Ok("Sources generated successfully.");
        }

        [Route("generate/random/suppliers")]
        public async Task<IActionResult> GenerateRandomSupplierAction()
        {
            await GenerateRandomSupplier();
            return Ok("Suppliers generated successfully.");
        }

        [Route("generate/random/uoms")]
        public async Task<IActionResult> GenerateRandomUOmAction()
        {
            await GenerateRandomUOm();
            return Ok("Units of measure generated successfully.");
        }

        [Route("generate/mutcd")]
        public async Task<IActionResult> GenerateMUTCDAction()
        {
            await GenerateRandomMUTCD();
            return Ok("MUTCD data generated successfully.");
        }
        [Route("generate/assetTypesAndAssociations")]
        public async Task<IActionResult> GenerateAssetTypesAndAssociationsAction()
        {
            await GenerateAssetTypesAndAssociations();
            return Ok("MUTCD data generated successfully.");
        }

        public IActionResult ImportAssetExcelSheet()
        {
            var model = new ExcelFileVM();
            return View("~/Views/Asset/ImportExcelSheet.cshtml", model);
        }
        [HttpPost]
        public async Task<ActionResult> ImportAssetExcelSheet(ExcelFileVM model)
        {
            if (model.File != null)
            {
                if (await _assetService.InitializeExcelData(model))
                {
                    return RedirectToAction("Index", "Asset");
                }
            }

            return RedirectToAction("ImportAssetExcelSheet");
        }


        public async Task GenerateRandomManufacturers()
        {
            var manufacturers = new List<string>()
            {
                "Siemens", "Econolite", "Trafficware", "Carmanah Technologies", "Tapco",
                "Polara Engineering", "Cooper Lighting Solutions", "Acuity Brands", "Signify",
                "Hubbell", "3M", "Avery Dennison", "Brady", "Vulcan Signs", "TrafFix Devices",
                "Plasticade", "Cortina Safety Products", "RoadSafe Traffic Systems", "Graco",
                "Titan Tool", "RAE Systems", "Ennis-Flint", "Crafco", "Schwarze Industries",
                "Falcon Asphalt Repair Equipment", "Bergkamp Inc.", "Bergkamp Inc.", "Multiquip",
                "Allen Engineering Corporation", "Power Curbers", "Miller Formless", "Husqvarna",
                "Bosch", "Makita", "Marshalltown", "Stihl", "Husqvarna", "Echo", "John Deere"
            };

            foreach (var manufacturer in manufacturers)
            {
                await _manufacturerService.Create(new ManufacturerModifyViewModel { Name = manufacturer });
            }
        }

        public async Task GenerateRandomConditions(int min = 1, int max = 3)
        {
            await _conditionService.Create(new ConditionModifyViewModel { Name = "Fair", Color = "orange" });
            await _conditionService.Create(new ConditionModifyViewModel { Name = "Good", Color = "green" });
            await _conditionService.Create(new ConditionModifyViewModel { Name = "Needs Replacement", Color = "red" });
            await _conditionService.Create(new ConditionModifyViewModel { Name = "Out Of Service ", Color = "grey" });

        }

        public async Task GenerateRandomCraftSkills(int min = 1, int max = 5)
        {
            var craftNames = new List<string>() { "Carpentry", "Masonry", "Plumbing", "Electrical Work", "Painting", "Roofing", "Flooring Installation", "Welding", "Concrete Work", "Drywall Installation" };
            var result = new List<CraftSkillModifyViewModel>();
            var random = Random.Shared; // Create a random instance
            var count = random.Next(min, max + 1); // Generate random count between min and max (inclusive)

            for (int i = 0; i < count; i++)
            {
                var craftIndex = random.Next(craftNames.Count); // Get random index for craft name
                var stRate = random.NextDouble() * 10; // Generate random ST rate between 0 and 10
                var otRate = random.NextDouble() * 10; // Generate random OT rate between 0 and 10
                var dtRate = random.NextDouble() * 10; // Generate random DT rate between 0 and 10

                await _craftSkillService.Create(new CraftSkillModifyViewModel
                {
                    Name = craftNames[craftIndex],
                    STRate = stRate,
                    OTRate = otRate,
                    DTRate = dtRate
                });
            }
        }
        public async Task GenerateRandomCategories(int min = 1, int max = 3)
        {
            var categoryNames = new List<string>() { "Traffic Signals", "School Flashers", "Streetlights", "Signs", "Traffic Safety", "Pavement Markings", "Pothole Repair", "Curb and Gutter", "Sidewalk Repair", "Vegetation Control" };
            foreach (var category in categoryNames)
            {
                await _categoryService.Create(new CategoryModifyViewModel
                {
                    Name = category
                });
            }

        }
        public async Task GenerateRandomLocation(int min = 1, int max = 3)
        {
            var locations = new List<string>()
            {
                "R1-C1-S1", "R1-C1-S2", "R1-C1-S3", "R1-C2-S1", "R1-C2-S2", "R1-C2-S3",
                "R1-C3-S1", "R1-C3-S2", "R1-C3-S3", "R2-C31-S1", "R2-C1-S2", "R2-C1-S3",
                "R2-C2-S1", "R2-C2-S2", "R2-C2-S3", "R2-C3-S1", "R2-C3-S2", "R2-C3-S3"
            };
            foreach (var location in locations)
            {
                await _locationService.Create(new LocationModifyViewModel
                {
                    Name = location
                });
            }

        }

        public async Task GenerateRandomSource(int min = 1, int max = 3)
        {
            var sources = new List<string>() { "General Fund", "Grant Funding", "Gas Tax Revenue", "Bond Proceeds", "Impact Fees", "Sales Tax", "State Aid", "Federal Aid", "Emergency Fund" };
            foreach (var source in sources)
            {
                await _sourceService.Create(new SourceModifyViewModel
                {
                    Name = source
                });
            }

        }

        public async Task GenerateRandomSupplier(int min = 1, int max = 3)
        {
            var supplierNames = new List<string>() { "Smith's Marketplace Hardware", "Home Depot", "ACE Hardware", "Los Alamos Building Supply", "Pajarito Builders Supply", "Los Alamos Concrete Products", "Los Alamos Electric Supply", "CB Electric Supply", "Mountain Paving", "Lowe's", "Fastenal", "Grainger", "HD Supply", "MSC Industrial Supply", "Graybar", "Rexel USA", "Vulcan Materials Company", "Granite Construction", "TruGreen", "BrightView" };
            foreach (var supplier in supplierNames)
            {
                await _suppplierService.Create(new SupplierModifyViewModel
                {
                    Name = supplier
                });
            }

        }
        public async Task GenerateRandomUOm(int min = 1, int max = 3)
        {
            var uoms = new List<string>() { "Smith's Marketplace Hardware", "Home Depot", "ACE Hardware", "Los Alamos Building Supply", "Pajarito Builders Supply", "Los Alamos Concrete Products", "Los Alamos Electric Supply", "CB Electric Supply", "Mountain Paving", "Lowe's", "Fastenal", "Grainger", "HD Supply", "MSC Industrial Supply", "Graybar", "Rexel USA", "Vulcan Materials Company", "Granite Construction", "TruGreen", "BrightView" };
            foreach (var uom in uoms)
            {
                await _uomService.Create(new UOMModifyViewModel
                {
                    Name = uom
                });
            }

        }

        public async Task GenerateRandomMUTCD()
        {
            List<MUTCDModifyViewModel> mutcds = new List<MUTCDModifyViewModel>();
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "mutcd.csv"); ;
            using (var reader = new StreamReader(filePath))
            {
                string line;
                var index = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (index != 0)
                    {
                        string[] fields = line.Split(',');
                        // Parse fields and populate your model
                        var item = new MUTCDModifyViewModel
                        {
                            Code = fields[0],
                            Description = fields[1],
                        };
                        mutcds.Add(item);
                    }

                    index++;
                }
            }
            foreach (var mutcd in mutcds)
            {
                mutcd.ImageUrl = $"/Storage/Assets/Others/{mutcd.Code}/{mutcd.Code}.png";
                await _mutcdService.Create(mutcd);
            }
        }

        public async Task GenerateAssetTypesAndAssociations()
        {
            var assetDetails = new Dictionary<string, Dictionary<string, List<string>>>
        {
            { "SIGNS", new Dictionary<string, List<string>>
                {
                    { "Sign Type", new List<string> { "None", "S1-1", "SW24-1", "SW24-2", "SW24-3", "S4-3", "W16-9", "W16-7P (R/L)", "S4-1, S4-5" } },
                    { "Post Type", new List<string> { "None", "Wood", "Metal Round", "Metal Square (telespar)", "Light post" } },
                    { "Post Location Type", new List<string> { "None", "Concrete", "Dirt", "Parkway", "Median" } },
                    { "Mount Type", new List<string> { "None", "Brackets", "Banding" } },
                    { "Hardware", new List<string> { "None", "Bolt", "Rivet", "Brace" } },
                    { "Dimension", new List<string> { "None", "30”X30”", "36”x36”", "30”x42”", "36”x48”", "24”x12”", "36”x12”", "30”x18”", "24”x48”", "30’x10’" } }
                }
            },
            { "STREET LIGHTS", new Dictionary<string, List<string>>
                {
                    { "Sign Type", new List<string> { "None" } }, // Add appropriate values for STREET LIGHTS if any
                    { "Post Type", new List<string> { "None", "Wood", "Metal Round", "Metal Square (telespar)", "Light post" } },
                    { "Post Location Type", new List<string> { "None", "Concrete", "Dirt", "Parkway", "Median" } },
                    { "Mount Type", new List<string> { "None", "Brackets", "Banding" } },
                    { "Hardware", new List<string> { "None", "Bolt", "Rivet", "Brace" } },
                    { "Dimension", new List<string> { "None", "30”X30”", "36”x36”", "30”x42”", "36”x48”", "24”x12”", "36”x12”", "30”x18”", "24”x48”", "30’x10’" } }
                }
            },
            { "TRAFFIC SIGNALS", new Dictionary<string, List<string>>
                {
                    { "Sign Type", new List<string> { "None" } }, // Add appropriate values for TRAFFIC SIGNALS if any
                    { "Post Type", new List<string> { "None", "Wood", "Metal Round", "Metal Square (telespar)", "Light post" } },
                    { "Post Location Type", new List<string> { "None", "Concrete", "Dirt", "Parkway", "Median" } },
                    { "Mount Type", new List<string> { "None", "Brackets", "Banding" } },
                    { "Hardware", new List<string> { "None", "Bolt", "Rivet", "Brace" } },
                    { "Dimension", new List<string> { "None", "30”X30”", "36”x36”", "30”x42”", "36”x48”", "24”x12”", "36”x12”", "30”x18”", "24”x48”", "30’x10’" } }
                }
            }
        };
            foreach (var assetType in assetDetails)
            {
                var assetTypeResponse = await _assetTypeService.Create(new AssetTypeModifyViewModel { Name = assetType.Key, Color = "#000", ActiveStatus = Enums.ActiveStatus.Active });
                var assetTypeId = (assetTypeResponse as RepositoryResponseWithModel<long>).ReturnModel;
                foreach (var level1 in assetType.Value)
                {
                    var assetTypeLevel1Response = await _assetTypeLevel1Service.Create(new AssetTypeLevel1ModifyViewModel { Name = level1.Key, AssetType = new AssetTypeBriefViewModel { Id = assetTypeId }, ActiveStatus = Enums.ActiveStatus.Active });
                    var assetTypeLevelId = (assetTypeLevel1Response as RepositoryResponseWithModel<long>).ReturnModel;
                    foreach (var value in level1.Value)
                    {
                        await _assetTypeLevel2Service.Create(new AssetTypeLevel2ModifyViewModel { Name = value, AssetTypeLevel1 = new AssetTypeLevel1BriefViewModel { Id = assetTypeLevelId }, AssetTypeId = assetTypeId, ActiveStatus = Enums.ActiveStatus.Active });
                    }
                }
            }

        }

    }
}
