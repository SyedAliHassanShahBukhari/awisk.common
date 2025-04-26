using FluentMigrator;

namespace awisk.common.Extensions
{
    public class ExceptionLogMigration : Migration
    {
        private readonly string tableName = "ExceptionLogs";
        public override void Up()
        {
            Create.Table(tableName).WithColumn("LogId").AsInt64().NotNullable().PrimaryKey().Identity().WithColumnDescription("Primary Key of the table")
                .WithColumn("Message").AsCustom("Text").NotNullable().WithDefaultValue("")
                .WithColumn("StackTrace").AsCustom("Text").NotNullable().WithDefaultValue("")
                .WithColumn("Type").AsCustom("Text").NotNullable().WithDefaultValue("")
                .WithColumn("URL").AsVarChar(100).NotNullable().WithDefaultValue("")
                .WithColumn("CreatedOn").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime).WithColumnDescription("");
        }
        public override void Down()
        {
            Delete.Table(tableName);
        }
    }
}
