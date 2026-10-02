using FluentMigrator.Builders.Create.Table;
using FluentMigrator;

namespace awisk.common.Extensions
{
    public static partial class MigrationExtensions
    {
        /// <summary>
        /// Adds the audit columns every entity base class declares. The column definitions here must
        /// stay in step with BaseEntity/BaseGuidEntity/BaseIntEntity/BaseLongEntity: UpdatedOn,
        /// UpdatedBy, DeletedOn and DeletedBy are nullable, and NULL means "never updated/deleted".
        /// Do not reintroduce a sentinel default — a NOT NULL column with a 1900-01-01 default
        /// rejects the explicit NULL Dapper.Contrib sends on insert and breaks every insert.
        /// </summary>
        public static ICreateTableWithColumnSyntax WithDefaultColumns(this ICreateTableWithColumnSyntax table)
        {
            if (table == null) throw new ArgumentException("table param can not be null");
            return table
                .WithColumn("CreatedOn").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("CreatedBy").AsString(100).NotNullable().WithDefaultValue("System")
                .WithColumn("UpdatedOn").AsDateTime2().Nullable()
                .WithColumn("UpdatedBy").AsString(100).Nullable()
                .WithColumn("DeletedOn").AsDateTime2().Nullable()
                .WithColumn("DeletedBy").AsString(100).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
