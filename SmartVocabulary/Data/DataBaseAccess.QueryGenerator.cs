using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Data
{
    /// <summary>
    /// Partial class to create SQL Queries
    /// </summary>
    public partial class DatabaseAccess
    {
        internal string GenerateCreateTableQuery(CultureInfo culture)
        {
            string tableName = culture.NativeName.Replace('(', '_')
                                                         .Replace(')', '_')
                                                         .Replace(' ', '_');
            StringBuilder createTable = new StringBuilder();
            createTable.Append("CREATE TABLE IF NOT EXISTS");
            createTable.Append("\"" + tableName + "\" "); // Result: "tableName" // notice the whitespace on the end
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
    }
}
