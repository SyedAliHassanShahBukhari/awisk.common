using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Table;

namespace awisk.common.Extensions
{
    public static partial class DataTypes
    {
        public static ICreateColumnOptionSyntax AsVarCharMax(this ICreateColumnAsTypeOrInSchemaSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsCustom("varchar(MAX)");
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsVarCharMax(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            string dataType = $"varchar(MAX)";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterTableAddColumnOrAlterColumnSyntax AsVarCharMax(this IAlterTableColumnAsTypeSyntax alterTableColumnAsTypeSyntax)
        {
            return alterTableColumnAsTypeSyntax.AsCustom("varchar(MAX)");
        }

        public static IAlterColumnOptionSyntax AsVarCharMax(this IAlterColumnAsTypeOrInSchemaSyntax alterTableColumnAsTypeSyntax)
        {
            return alterTableColumnAsTypeSyntax.AsCustom("varchar(MAX)");
        }

        //AsVarChar(size)        
        public static ICreateColumnOptionSyntax AsVarChar(this ICreateColumnAsTypeOrInSchemaSyntax createTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"varchar({size})";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsVarChar(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"varchar({size})";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AsVarChar(this IAlterTableColumnAsTypeSyntax alterTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"varchar({size})";
            return alterTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterColumnOptionSyntax AsVarChar(this IAlterColumnAsTypeOrInSchemaSyntax alterTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"varchar({size})";
            return alterTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        // AsNVarCharMax
        public static ICreateColumnOptionSyntax AsNVarCharMax(this ICreateColumnAsTypeOrInSchemaSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsCustom("nvarchar(MAX)");
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsNVarCharMax(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            string dataType = $"nvarchar(MAX)";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AsNVarCharMax(this IAlterTableColumnAsTypeSyntax alterTableColumnAsTypeSyntax)
        {
            return alterTableColumnAsTypeSyntax.AsCustom("nvarchar(MAX)");
        }

        public static IAlterColumnOptionSyntax AsNVarCharMax(this IAlterColumnAsTypeOrInSchemaSyntax alterTableColumnAsTypeSyntax)
        {
            return alterTableColumnAsTypeSyntax.AsCustom("nvarchar(MAX)");
        }

        // AsNVarChar(size)
        public static ICreateColumnOptionSyntax AsNVarChar(this ICreateColumnAsTypeOrInSchemaSyntax createTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"nvarchar({size})";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsNVarChar(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"nvarchar({size})";
            return createTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AsNVarChar(this IAlterTableColumnAsTypeSyntax alterTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"nvarchar({size})";
            return alterTableColumnAsTypeSyntax.AsCustom(dataType);
        }

        public static IAlterColumnOptionSyntax AsNVarChar(this IAlterColumnAsTypeOrInSchemaSyntax alterTableColumnAsTypeSyntax, int size)
        {
            string dataType = $"nvarchar({size})";
            return alterTableColumnAsTypeSyntax.AsCustom(dataType);
        }
    }
}
