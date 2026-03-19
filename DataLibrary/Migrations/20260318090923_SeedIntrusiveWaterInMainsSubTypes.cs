using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedIntrusiveWaterInMainsSubTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed: Incident Type "Intrusive Water in Mains" + its Sub-Types
            // Uses name-based lookups and IF NOT EXISTS to be idempotent.
            migrationBuilder.Sql(@"
DECLARE @typeName NVARCHAR(200) = N'Intrusive Water in Mains';
DECLARE @typeId BIGINT;

-- Ensure parent type exists in EventTypes
SELECT @typeId = Id FROM EventTypes WHERE IsDeleted = 0 AND Name = @typeName;
IF (@typeId IS NULL)
BEGIN
    INSERT INTO EventTypes (Name, Description, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
    VALUES (@typeName, N'', 0, 1, SYSUTCDATETIME(), 0, SYSUTCDATETIME(), 0);

    SELECT @typeId = SCOPE_IDENTITY();
END

-- Sub-Types
IF (@typeId IS NOT NULL)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM EventSubTypes WHERE IsDeleted = 0 AND EventTypeId = @typeId AND Name = N'No gas possibly related to water')
        INSERT INTO EventSubTypes (EventTypeId, Name, Description, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
        VALUES (@typeId, N'No gas possibly related to water', N'', 0, 1, SYSUTCDATETIME(), 0, SYSUTCDATETIME(), 0);

    IF NOT EXISTS (SELECT 1 FROM EventSubTypes WHERE IsDeleted = 0 AND EventTypeId = @typeId AND Name = N'Low gas possibly related to water')
        INSERT INTO EventSubTypes (EventTypeId, Name, Description, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
        VALUES (@typeId, N'Low gas possibly related to water', N'', 0, 1, SYSUTCDATETIME(), 0, SYSUTCDATETIME(), 0);

    IF NOT EXISTS (SELECT 1 FROM EventSubTypes WHERE IsDeleted = 0 AND EventTypeId = @typeId AND Name = N'Water seen at meter or gas equipment')
        INSERT INTO EventSubTypes (EventTypeId, Name, Description, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
        VALUES (@typeId, N'Water seen at meter or gas equipment', N'', 0, 1, SYSUTCDATETIME(), 0, SYSUTCDATETIME(), 0);

    IF NOT EXISTS (SELECT 1 FROM EventSubTypes WHERE IsDeleted = 0 AND EventTypeId = @typeId AND Name = N'Suspected water intrusion')
        INSERT INTO EventSubTypes (EventTypeId, Name, Description, IsDeleted, ActiveStatus, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
        VALUES (@typeId, N'Suspected water intrusion', N'', 0, 1, SYSUTCDATETIME(), 0, SYSUTCDATETIME(), 0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @typeName NVARCHAR(200) = N'Intrusive Water in Mains';
DECLARE @typeId BIGINT;

SELECT @typeId = Id FROM EventTypes WHERE Name = @typeName;
IF (@typeId IS NOT NULL)
BEGIN
    DELETE FROM EventSubTypes
    WHERE EventTypeId = @typeId
      AND Name IN (
        N'No gas possibly related to water',
        N'Low gas possibly related to water',
        N'Water seen at meter or gas equipment',
        N'Suspected water intrusion'
      );
END
");
        }
    }
}
