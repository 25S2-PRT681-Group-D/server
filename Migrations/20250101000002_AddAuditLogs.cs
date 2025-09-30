using FluentMigrator;

namespace AgroScan.API.Migrations
{
    [Migration(20250101000002)]
    public class AddAuditLogs : Migration
    {
        public override void Up()
        {
            Create.Table("AuditLogs")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("UserId").AsString(50).Nullable()
                .WithColumn("Action").AsString(100).NotNullable()
                .WithColumn("EntityType").AsString(100).NotNullable()
                .WithColumn("EntityId").AsString(50).NotNullable()
                .WithColumn("OldValues").AsString(int.MaxValue).Nullable()
                .WithColumn("NewValues").AsString(int.MaxValue).Nullable()
                .WithColumn("IpAddress").AsString(45).Nullable()
                .WithColumn("UserAgent").AsString(500).Nullable()
                .WithColumn("Timestamp").AsDateTime().NotNullable();

            Create.Index("IX_AuditLogs_UserId")
                .OnTable("AuditLogs")
                .OnColumn("UserId");

            Create.Index("IX_AuditLogs_Action")
                .OnTable("AuditLogs")
                .OnColumn("Action");

            Create.Index("IX_AuditLogs_EntityType")
                .OnTable("AuditLogs")
                .OnColumn("EntityType");

            Create.Index("IX_AuditLogs_Timestamp")
                .OnTable("AuditLogs")
                .OnColumn("Timestamp");
        }

        public override void Down()
        {
            Delete.Table("AuditLogs");
        }
    }
}
