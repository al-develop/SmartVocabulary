using System.Globalization;
using System.Text;

namespace SmartVocabulary.Data
{
    /// <summary>
    /// Partial class to create Private SQL Queries
    /// </summary>
    public partial class DatabaseAccess
    {
        /// <summary>
        /// Generates a SQL Query as System.String which creates a table with the passed Parameter
        /// </summary>
        /// <param name="culture">The CultureInfo object which Native Name is used as a table name</param>
        /// <returns>A System.String with the SQL query</returns>
        private string GenerateCreateTableQuery(CultureInfo culture)
        {
            StringBuilder createTable = new StringBuilder();
            createTable.Append("CREATE TABLE IF NOT EXISTS");
            createTable.Append("[" + GenerateTableName(culture) + "] "); // Result: [tableName] // notice the whitespace on the end
            createTable.Append(@"(
                        [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Native] NVARCHAR,
                        [Translation] NVARCHAR,
                        [Definition] NVARCHAR,
                        [Kind] NVARCHAR,
                        [Synonym] NVARCHAR,
                        [Opposite] NVARCHAR,
                        [Example] NVARCHAR
                    );");

            return createTable.ToString().Replace('\r', ' ').Replace('\n', ' ');
        }

        private string GenerateCreateTableQuery(string tableName)
        {
            StringBuilder createTable = new StringBuilder();
            createTable.Append("CREATE TABLE IF NOT EXISTS");
            createTable.Append("[" + GenerateTableName(tableName) + "] "); // Result: [tableName] // notice the whitespace on the end
            createTable.Append(@"(
                        [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Native] NVARCHAR,
                        [Translation] NVARCHAR,
                        [Definition] NVARCHAR,
                        [Kind] NVARCHAR,
                        [Synonym] NVARCHAR,
                        [Opposite] NVARCHAR,
                        [Example] NVARCHAR
                    );");

            return createTable.ToString().Replace('\r', ' ').Replace('\n', ' ');
        }


        private string GenerateInsertQuery(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append(GenerateTableName(tableName));

            // Columns
            builder.Append("(");
            builder.Append("Native, ");
            builder.Append("Translation, ");
            builder.Append("Definition, ");
            builder.Append("Kind, ");
            builder.Append("Synonym, ");
            builder.Append("Opposite, ");
            builder.Append("Example");
            builder.Append(")");

            // Values
            builder.Append(" VALUES");
            builder.Append("(");
            builder.Append("@native, ");
            builder.Append("@translation, ");
            builder.Append("@definition, ");
            builder.Append("@kind, ");
            builder.Append("@synonym, ");
            builder.Append("@opposite, ");
            builder.Append("@example");
            builder.Append(")");

            return builder.ToString().Replace('\r', ' ').Replace('\n', ' ');
        }

        private string GenerateSelectAllQuery(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT * FROM ");
            builder.Append(GenerateTableName(tableName));

            return builder.ToString();
        }

        private string GenerateDeleteQuery(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append(GenerateTableName(tableName));
            builder.Append(" WHERE ID = @id");
            return builder.ToString();
        }

        private string GenerateUpdateQuery(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append(GenerateTableName(tableName));
            builder.Append(" SET ");

            // Columns
            builder.Append("Native = @native, ");
            builder.Append("Translation = @translation, ");
            builder.Append("Definition = @definition, ");
            builder.Append("Kind = @kind, ");
            builder.Append("Synonym = @synonym, ");
            builder.Append("Opposite = @opposite, ");
            builder.Append("Example = @example");

            // Condition
            builder.Append(" WHERE ID = @id");

            return builder.ToString();
        }

        #region Static methods to Generate table names
        private static string GenerateTableName(CultureInfo culture) 
            => culture.NativeName.Replace('(', '_')
            .Replace(')', '_')
            .Replace(' ', '_');

        private static string GenerateTableName(string tableName)
            => tableName.Replace('(', '_')
            .Replace(')', '_')
            .Replace(' ', '_');

        #endregion Static methods to Generate table names
    }
}