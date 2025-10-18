using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TaskWorkStep : BaseDBModel
    {
        public string Title { get; set; }
        public int Order { get; set; }

        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType { get; set; }

    }
}
