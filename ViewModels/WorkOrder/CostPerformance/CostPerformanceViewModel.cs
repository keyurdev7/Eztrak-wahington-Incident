using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Timesheet;

namespace ViewModels.WorkOrder.CostPerformance
{
    public class CostPerformanceViewModel
    {
        public List<TaskWorkStepViewModel> TaskWorkSteps { get; set; } = new();
        public List<TaskLaborViewModel> TaskLabors { get; set; } = new();
        public List<TaskMaterialViewModel> TaskMaterials { get; set; } = new();
        public List<TaskEquipmentViewModel> TaskEquipments { get; set; } = new();
        public List<EquipmentTransactionDetailViewModel> EquipmentTransactions { get; set; } = new();
        public List<TransactionDetailViewModel> MaterialTransactions { get; set; } = new();
        public List<TimesheetBriefViewModel> TimeSheets { get; set; } = new();
        public List<CostPerformanceMaterialViewModel> MaterialsCostPerformance { get; set; } = new();
        public List<CostPerformanceEquipmentViewModel> EquipmentsCostPerformance { get; set; } = new();
        public List<CostPerformanceLaborViewModel> LaborsCostPerformance { get; set; } = new();
        public List<CostPerformanceLaborViewModel> ActualLaborsCostPerformance { get; set; } = new();

        public void SetMaterialCostPerformance()
        {
            var costPerformance = MaterialTransactions
                .GroupBy(x => x.Inventory.Id)
                .Select(x => new CostPerformanceMaterialViewModel
                {
                    ActualCost = Math.Abs(x.Sum(y => y.Quantity * y.ItemPrice)),
                    Material = new InventoryBriefViewModel
                    {
                        Id = x.Max(y => y.Inventory.Id),
                        ItemNo = x.Max(y => y.Inventory.ItemNo),
                        Description = x.Max(y => y.Inventory.Description),
                    },
                    Quantity = Math.Abs(x.Sum(y => y.Quantity)),
                    UOM = x.Max(y => y.Inventory.UOM.Name),
                }).ToList();
            //foreach (var material in TaskMaterials)
            //{
            //    var cp = costPerformance.Where(x => x.Material.Id == material.Id).FirstOrDefault();
            //    if (cp == null)
            //    {
            //        costPerformance.Add(new CostPerformanceMaterialViewModel
            //        {
            //            Cost = material.Cost,
            //            Material = new InventoryBriefViewModel
            //            {
            //                Id = material.Material.Id,
            //                Description = material.Material.Description,
            //            },
            //        });
            //    }
            //    else
            //    {
            //        cp.Cost = material.Cost;
            //    }
            //}
            MaterialsCostPerformance = costPerformance;

        }

        public void SetEquipmentCostPerformance()
        {
            var costPerformance = EquipmentTransactions.Where(x => x.TransactionType == Enums.EquipmentTransactionTypeCatalog.Return)
                .GroupBy(x => x.Equipment.Id)
                .Select(x => new CostPerformanceEquipmentViewModel
                {
                    Equipment = new EquipmentBriefViewModel
                    {
                        Id = x.Max(y => y.Equipment.Id),
                        Description = x.Max(y => y.Equipment.Description),
                    },
                    HourlyRate = x.Average(y => y.HourlyRate),
                    Quantity = x.Sum(y => y.Quantity),
                    Hours = x.Sum(y => y.Hours),
                    ActualCost = Math.Abs(x.Sum(y => y.Quantity * y.Hours * y.HourlyRate)),
                }).ToList();
            //foreach (var equipment in TaskEquipments)
            //{
            //    var cp = costPerformance.Where(x => x.Equipment.Id == equipment.Id).FirstOrDefault();
            //    if (cp == null)
            //    {
            //        costPerformance.Add(new CostPerformanceEquipmentViewModel
            //        {
            //            Cost = equipment.Cost,
            //            Equipment = new EquipmentBriefViewModel
            //            {
            //                Id = equipment.Equipment.Id,
            //                Description = equipment.Equipment.Description,
            //            }
            //        });
            //    }
            //    else
            //    {
            //        cp.Cost = equipment.Cost;
            //    }
            //}
            EquipmentsCostPerformance = costPerformance;
        }

        public void SetLaborCostPerformance()
        {
            var costPerformance = TimeSheets
                .Where(x => x.Craft != null)
                .GroupBy(x => x.Craft.Id)
                .Select(x => new CostPerformanceLaborViewModel
                {
                    ActualCost = Math.Abs(x.Sum(y => y.TotalCost)),
                    CraftSkill = new CraftSkillBriefViewModel
                    {
                        Id = x.Max(y => y.Craft.Id),
                        Name = x.Max(y => y.Craft.Name),
                    },
                    ActualHours = x.Sum(x => x.TotalHours)
                }).ToList() ?? new();
            ActualLaborsCostPerformance = costPerformance;
            //foreach (var tl in TaskLabors)
            //{
            //    var cp = costPerformance.Where(x => x.CraftSkill.Id == tl.CraftSkill.Id).FirstOrDefault();
            //    if (cp == null)
            //    {
            //        costPerformance.Add(new CostPerformanceLaborViewModel
            //        {
            //            Cost = tl.Total,
            //            CraftSkill = new CraftSkillBriefViewModel
            //            {
            //                Id = tl.CraftSkill.Id,
            //                Name = tl.CraftSkill.Name,
            //            },
            //            Hours = tl.Hours,
            //        });
            //    }
            //    else
            //    {
            //        cp.Cost = tl.Total;
            //        cp.Hours = tl.Hours;
            //    }
            //}
            LaborsCostPerformance = costPerformance;
        }

    }
}
