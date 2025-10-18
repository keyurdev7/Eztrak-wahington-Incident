using Helpers.Datetime;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Report.RawReport
{
    public class AssetRawReportViewModel
    {
        public long Id { get; set; }
        public string SystemGeneratedId { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public string AssetTypeName { get; set; }
        public string MUTCDCode { get; set; }

        public DateTime DateAdded { get; set; }
        public string FormattedDateAdded
        {
            get
            {
                return DateAdded.FormatDate();
            }
        }
        public DateTime NextMaintenanceDate { get; set; }
        public string FormattedNextMaintenanceDate
        {
            get
            {
                return NextMaintenanceDate.FormatDate();
            }
        }

        public string FormattedReplacementDate
        {
            get
            {
                return ReplacementDate.FormatDate();
            }
        }
        public DateTime ReplacementDate { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime LastReplacementDate { get; set; }
        public DateTime LastRepairDate { get; set; }
        public DateTime LastRemoveDate { get; set; }

        public string FormattedLastMaintenanceDate
        {
            get
            {
                return LastMaintenanceDate.FormatDate();
            }
        }
        public string FormattedLastReplacementDate
        {
            get
            {
                return LastReplacementDate.FormatDate();
            }
        }
        public string FormattedLastRepairDate
        {
            get
            {
                return LastRepairDate.FormatDate();
            }
        }
        public string FormattedLastRemovalDate
        {
            get
            {
                return LastRemoveDate.FormatDate();
            }
        }
        public string SignType { get; set; }
        public string PostType { get; set; }
        public string PostLocationType { get; set; }
        public string MountType { get; set; }
        public string Hardware { get; set; }
        public string Dimension { get; set; }
        public Dictionary<string, string> Associations { get; set; } = new();
        //Sign Type   Post type   Post location type Mount type Hardware
    }

    public class AssetAssociationRawReportViewModel
    {
        public long Id { get; }
        public long AssetId { get; }
        public long AssetLevel1Id { get; }
        public string AssetLevel1Name { get; }
        public long AssetLevel2Id { get; }
        public string AssetLevel2Name { get; }

        public AssetAssociationRawReportViewModel(long id, long assetId, long assetLevel1Id, string assetLevel1Name, long assetLevel2Id, string assetLevel2Name)
        {
            Id = id;
            AssetId = assetId;
            AssetLevel1Id = assetLevel1Id;
            AssetLevel1Name = assetLevel1Name;
            AssetLevel2Id = assetLevel2Id;
            AssetLevel2Name = assetLevel2Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is AssetAssociationRawReportViewModel other &&
                   Id == other.Id &&
                   AssetId == other.AssetId &&
                   EqualityComparer<long>.Default.Equals(AssetLevel1Id, other.AssetLevel1Id) &&
                   AssetLevel1Name == other.AssetLevel1Name &&
                   EqualityComparer<long>.Default.Equals(AssetLevel2Id, other.AssetLevel2Id) &&
                   AssetLevel2Name == other.AssetLevel2Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, AssetId, AssetLevel1Id, AssetLevel1Name, AssetLevel2Id, AssetLevel2Name);
        }
    }
}
