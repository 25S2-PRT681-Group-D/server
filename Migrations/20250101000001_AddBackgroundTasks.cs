using FluentMigrator;

namespace AgroScan.API.Migrations
{
    [Migration(20250101000001)]
    public class AddBackgroundTasks : Migration
    {
        public override void Up()
        {
            Create.Table("BackgroundTasks")
                .WithColumn("Id").AsString(50).PrimaryKey()
                .WithColumn("TaskName").AsString(100).NotNullable()
                .WithColumn("TaskData").AsString(int.MaxValue).NotNullable()
                .WithColumn("Status").AsString(20).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("ScheduledAt").AsDateTime().NotNullable()
                .WithColumn("StartedAt").AsDateTime().Nullable()
                .WithColumn("CompletedAt").AsDateTime().Nullable()
                .WithColumn("Attempts").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("MaxAttempts").AsInt32().NotNullable().WithDefaultValue(3)
                .WithColumn("ErrorMessage").AsString(1000).Nullable();

            Create.Index("IX_BackgroundTasks_Status")
                .OnTable("BackgroundTasks")
                .OnColumn("Status");

            Create.Index("IX_BackgroundTasks_ScheduledAt")
                .OnTable("BackgroundTasks")
                .OnColumn("ScheduledAt");

            Create.Index("IX_BackgroundTasks_CreatedAt")
                .OnTable("BackgroundTasks")
                .OnColumn("CreatedAt");
        }

        public override void Down()
        {
            Delete.Table("BackgroundTasks");
        }
    }
}
