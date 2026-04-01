using DataLibrary;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260322103000_FixedSeverityLevelsData")]
    public class FixedSeverityLevelsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Canonical US incident severities; soft-delete legacy rows; remap FKs; insert fixed set.
            migrationBuilder.Sql(@"
DECLARE @UtcNow datetime2 = SYSUTCDATETIME();
DECLARE @By bigint = 1;

UPDATE SeverityLevels SET IsDeleted = 1, UpdatedOn = @UtcNow, UpdatedBy = @By WHERE IsDeleted = 0;

INSERT INTO SeverityLevels (Name, Description, Color, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
VALUES
(N'Severe', N'Life safety / major system impact', N'#DC3545', 0, 1, @UtcNow, @By, @UtcNow, @By),
(N'High', N'Major incident / emergency response', N'#FD7E14', 0, 1, @UtcNow, @By, @UtcNow, @By),
(N'Moderate', N'Localized hazard', N'#FFC107', 0, 1, @UtcNow, @By, @UtcNow, @By),
(N'Low', N'Routine service issue', N'#28A745', 0, 1, @UtcNow, @By, @UtcNow, @By),
(N'Non-Incident', N'Informational / not an incident', N'#6C757D', 0, 1, @UtcNow, @By, @UtcNow, @By);

DECLARE @ModerateId bigint = (SELECT TOP 1 Id FROM SeverityLevels WHERE IsDeleted = 0 AND Name = N'Moderate' ORDER BY Id DESC);

UPDATE i SET i.SeverityLevelId = nh.Id
FROM Incidents i
INNER JOIN SeverityLevels oh ON oh.Id = i.SeverityLevelId AND oh.IsDeleted = 1
INNER JOIN SeverityLevels nh ON nh.IsDeleted = 0 AND LTRIM(RTRIM(LOWER(oh.Name))) = LTRIM(RTRIM(LOWER(nh.Name)))
WHERE i.SeverityLevelId IS NOT NULL;

UPDATE Incidents SET SeverityLevelId = @ModerateId
WHERE SeverityLevelId IS NOT NULL AND SeverityLevelId IN (SELECT Id FROM SeverityLevels WHERE IsDeleted = 1);

UPDATE iv SET iv.ConfirmedSeverityLevelId = nh.Id
FROM IncidentValidations iv
INNER JOIN SeverityLevels oh ON oh.Id = iv.ConfirmedSeverityLevelId AND oh.IsDeleted = 1
INNER JOIN SeverityLevels nh ON nh.IsDeleted = 0 AND LTRIM(RTRIM(LOWER(oh.Name))) = LTRIM(RTRIM(LOWER(nh.Name)));

UPDATE IncidentValidations SET ConfirmedSeverityLevelId = @ModerateId
WHERE ConfirmedSeverityLevelId IN (SELECT Id FROM SeverityLevels WHERE IsDeleted = 1);

UPDATE ivl SET ivl.ConfirmedSeverityLevelId = nh.Id
FROM IncidentValidationLocations ivl
INNER JOIN SeverityLevels oh ON oh.Id = ivl.ConfirmedSeverityLevelId AND oh.IsDeleted = 1
INNER JOIN SeverityLevels nh ON nh.IsDeleted = 0 AND LTRIM(RTRIM(LOWER(oh.Name))) = LTRIM(RTRIM(LOWER(nh.Name)))
WHERE ivl.ConfirmedSeverityLevelId IS NOT NULL;

UPDATE IncidentValidationLocations SET ConfirmedSeverityLevelId = @ModerateId
WHERE ConfirmedSeverityLevelId IS NOT NULL AND ConfirmedSeverityLevelId IN (SELECT Id FROM SeverityLevels WHERE IsDeleted = 1);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data migration — no safe automatic rollback.
        }
    }
}
