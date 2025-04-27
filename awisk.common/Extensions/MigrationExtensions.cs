using FluentMigrator.Builders.Create.Table;
using FluentMigrator;

namespace awisk.common.Extensions
{
    public static partial class MigrationExtensions
    {
        public static ICreateTableWithColumnSyntax WithDefaultColumns(this ICreateTableWithColumnSyntax table)
        {
            if (table == null) throw new ArgumentException("table param can not be null");
            return table
                .WithColumn("CreatedOn").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("CreatedBy").AsString(100).NotNullable().WithDefaultValue("System")
                .WithColumn("UpdatedOn").AsDateTime2().NotNullable().WithDefaultValue("1900-01-01")
                .WithColumn("UpdatedBy").AsString(100).Nullable()
                .WithColumn("DeletedOn").AsDateTime2().NotNullable().WithDefaultValue("1900-01-01")
                .WithColumn("DeletedBy").AsString(100).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
