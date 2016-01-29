using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SQLite;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using System.IO;
using System.Globalization;
using System.Data;

namespace SmartVocabulary.Data
{
    /// <summary>
    /// Data Layer. Work directly with SQL and the Database
    /// SQL Queries are located in the other part of the partial class (DatabaseAccess.QueryGenerator.cs)
    /// </summary>
    public partial class DatabaseAccess : IDisposable
    {
        private readonly string _connectionString;
        private readonly static string _saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private readonly string _savePath = String.Format("{0}\\{1}", _saveDir, "smartVocDb.sqlite");
        private SQLiteConnection _connection;

        public DatabaseAccess()
        {
            this._connectionString = String.Format("Data Source={0};Version=3", _savePath);
            _connection = new SQLiteConnection(_connectionString);

            if (!Directory.Exists(_saveDir))
                Directory.CreateDirectory(_saveDir);
            if (!File.Exists(_savePath))
                SQLiteConnection.CreateFile(_savePath);

            // The Method to create Tables should be called every time in the Constructor
            // It can happen, that the DB schema was changed (tables got deleted etc)
            // So, to keep it all working as it should, the tables are getting created every time, when this class is initalized
            // The "Create" - SQL Command checks already, if the table exists or not. (Query: CREATE IF NOT EXISTS)
            this.CreateTables();
        }

        private async void CreateTables()
        {
            List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
            int counterForFailure = 0;  // if an error occures, it's easier to find the object which causes the error whotud a counter
            try
            {
                foreach (CultureInfo culture in cultures)
                {
                    string query = GenerateCreateTableQuery(culture);
                    await Task.Run(() =>
                    {
                        using (SQLiteCommand createCommand = new SQLiteCommand(query, this._connection))
                        {
                            if (this._connection.State != ConnectionState.Open)
                                this._connection.Open();
                                                       
                            createCommand.ExecuteNonQuery();                            
                        }
                    });
                }
                    counterForFailure++;
            }
            catch (Exception ex)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.Append("Error occured on creating the Database Tables");
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.Append(cultures.ElementAt(counterForFailure));
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.Append(ex.Message);

                LogWriter.Instance.WriteLine(errorBuilder.ToString());
            }
            finally
            {
                if (this._connection.State != ConnectionState.Closed)
                    this._connection.Close();
            }
        }

        public Result<int> SaveVocable(Vocable vocable, string tableName)
        {
            int result = 0;
            try
            {
                SQLiteCommand com = new SQLiteCommand();
                using (SQLiteCommand command = new SQLiteCommand(GenerateInsertQuery(tableName), this._connection))
                {
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Native", ParameterName = "@native", Value = vocable.Native });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Translation", ParameterName = "@translation", Value = vocable.Translation });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Definition", ParameterName = "@definition", Value = vocable.Definition });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Kind", ParameterName = "@kind", Value = vocable.Kind.ToString() });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Synonym", ParameterName = "@synonym", Value = vocable.Synonym });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Opposite", ParameterName = "@opposite", Value = vocable.Opposite });
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "Example", ParameterName = "@example", Value = vocable.Example });

                    if (this._connection.State != ConnectionState.Open)
                        this._connection.Open();

                    command.ExecuteNonQuery();
                    //SQLiteCommand selectID = new SQLiteCommand(String.Format("SELECT ID FROM {0}",tableName), this._connection);
                    //var reader = selectID.ExecuteReader();
                    //var results = new List<object>();
                    //int i = 0;
                    //while(reader.Read())
                    //{
                    //    results.Add(reader[i]);
                    //    i++;
                    //}


                    //if (this._connection.State != ConnectionState.Closed)
                    //    this._connection.Close();
                    //return null;
                    //bool isSuccess = Int32.TryParse(resultSet.ToString(), out result);

                    //if (isSuccess)
                    return new Result<int>(-1, "", Status.Success);
                    //else
                    //    return new Result<int>(-1, "resultSet returned invalid result - DatabaseAccess.SaveVocable()", Status.Error);

                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine(String.Format("Error occured at SaveVocable in DataBaseAccess:\n{0}", ex.Message));
                return new Result<int>(ex.Message, "", Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != System.Data.ConnectionState.Closed)
                    this._connection.Close();
            }
        }

        public Result<Vocable> GetVocableById(int id)
        {

            return new Result<Vocable>();
        }

        public Result<List<Vocable>> GetAllVocables(string tableName)
        {
            var result = new List<Vocable>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(GenerateSelectAllQuery(tableName), this._connection))
                {
                    if (this._connection.State != ConnectionState.Open)
                        this._connection.Open();

                    SQLiteDataReader reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return new Result<List<Vocable>>(new List<Vocable>(), "empy database", Status.Success);

                    while (reader.Read())
                    {
                        var temp = new Vocable();
                        temp.ID = Vocable.SetIdDynamic(reader["ID"]);
                        temp.Kind = Vocable.ConvertStringToKind(reader["Kind"].ToString());
                        temp.Native = reader["Native"].ToString();
                        temp.Translation = reader["Translation"].ToString();
                        temp.Definition = reader["Definition"].ToString();
                        temp.Example = reader["Example"].ToString();
                        temp.Opposite = reader["Opposite"].ToString();
                        temp.Synonym = reader["Synonym"].ToString();

                        result.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine(String.Format("Error occured at \"GetAllVocables\" in DataBaseAccess:\n{0}", ex.Message));
                return new Result<List<Vocable>>(ex.Message, "", Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != ConnectionState.Closed)
                    this._connection.Close();
            }

            return new Result<List<Vocable>>(result, "", Status.Success);
        }

        #region IDisposable Member
        public void Dispose()
        {
            this._connection.Close();
        }
        #endregion
    }
}