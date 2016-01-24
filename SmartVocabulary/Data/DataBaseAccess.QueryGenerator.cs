using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            createTable.Append("\"" + GenerateTableName(culture) + "\" "); // Result: "tableName" // notice the whitespace on the end
            createTable.Append(@"(
                        [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Native] TEXT(50),
                        [Definition] TEXT(100),
                        [Translation] TEXT(50),
                        [Kind] TEXT(15),
                        [Synonym] TEXT(50),
                        [Opposite] TEXT(50),
                        [Example] TEXT(200)
                    )");

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

        private static string GenerateTableName(CultureInfo culture)
        {
            return culture.NativeName.Replace('(', '_')
                          .Replace(')', '_')
                          .Replace(' ', '_');
        }

        private static string GenerateTableName(string tableName)
        {
            return tableName.Replace('(', '_')
                            .Replace(')', '_')
                            .Replace(' ', '_');
        }

    }
}
