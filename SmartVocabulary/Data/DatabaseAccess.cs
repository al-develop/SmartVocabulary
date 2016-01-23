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

namespace SmartVocabulary.Data
{
    /// <summary>
    /// Data Layer. Work directly with SQL and the Database
    /// SQL Queries are located in the other part of the partial class (DatabaseAccess.QueryGenerator.cs)
    /// </summary>
    public partial class DatabaseAccess : IDisposable
    {
        private readonly string _connectionString;
        private static List<Vocable> _preLoadedList;
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
            // The "Create" - SQL Command checks already, if the table exists or not.
            this.GenerateTables();
        }

        /// <summary>
        /// Wrapper Method, to call "CreateTables" asynchronously
        /// </summary>
        private async void GenerateTables()
        {
            await this.CreateTables();
        }

        private async Task<Result> CreateTables()
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
                            this._connection.Open();
                            createCommand.ExecuteNonQuery();
                            this._connection.Close();
                        }
                    });
                    counterForFailure++;
                }
            }        
            catch(Exception ex)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.Append("Error occured on creating the Database Tables");
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.Append(cultures.ElementAt(counterForFailure));
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.Append(ex.Message);

                LogWriter.Instance.WriteLine(errorBuilder.ToString());
                return new Result(errorBuilder.ToString(), Status.Error, ex);
            }

            return new Result("", Status.Success);
        }

        public Result SaveVocable(Vocable vocable)
        {
                       
            return new Result();
        }

        public Result<Vocable> GetVocableById(int id)
        {
            if (_preLoadedList != null)
            {
                return new Result<Vocable>(_preLoadedList.First(f => f.ID == id));
            }

            return new Result<Vocable>();
        }

        public Result<List<Vocable>> GetAllVocables()
        {
            if (_preLoadedList != null)
            {
                return new Result<List<Vocable>>(_preLoadedList);
            }

            return new Result<List<Vocable>>();
        }

        #region IDisposable Member
        public void Dispose()
        {
            this._connection.Close();
        }
        #endregion
    }
}