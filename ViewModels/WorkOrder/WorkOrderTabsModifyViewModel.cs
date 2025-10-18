using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels.Manager;
using Microsoft.AspNetCore.Http;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrder;
using Helpers.Datetime;
using Helpers.File;
using Models;
using System.ComponentModel.DataAnnotations.Schema;
using ViewModels.CRUD;
using ViewModels.Shared.Notes;

namespace ViewModels
{
    public class WorkOrderTabsModifyViewModel 
    {
        public CrudUpdateViewModel CrudUpdateViewModel { get; set; }
        public List<INotesViewModel> Notes { get; set; }
        public WorkOrderDetailViewModel WorkOrderDetailModel { get; set; }
        public AssetDetailViewModel AssetDetailModel { get; set; }
    }


}
