﻿using System;
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
        private readonly string _savePath = $"{_saveDir}\\{"smartVocDb.sqlite"}";
        private SQLiteConnection _connection;

        public DatabaseAccess()
        {
            this._connectionString = $"Data Source={_savePath};Version=3";
            _connection = new SQLiteConnection(_connectionString);

            //if (!Directory.Exists(_saveDir))
            //    Directory.CreateDirectory(_saveDir);
            //if (!File.Exists(_savePath))
            //    SQLiteConnection.CreateFile(_savePath);

            // 31.01.2016 OBSOLETE: The Database should not be cerated in the Constructor anymore
            // as for now, it's created from the Settings
            //
            // The Method to create Tables should be called every time in the Constructor
            // It can happen, that the DB schema was changed (tables got deleted etc)
            // So, to keep it all working as it should, the tables are getting created every time, when this class is initalized
            // The "Create" - SQL Command checks already, if the table exists or not. (Query: CREATE IF NOT EXISTS)
            //this.CreateTables();
        }

        public async Task<Result> CreateNewDatabaseAsync()
        {
            LogWriter.Instance.WriteLine("Generating Database");
            await Task.Run(() => this.CreateNewDatabaseFile());
            return await this.CreateTablesAsync();
        }

        public async Task<Result> CreateTableAsync(string tableName)
        {
            try
            {
                string query = this.GenerateCreateTableQuery(tableName);
                await Task.Run(() =>
                {
                    using (SQLiteCommand createCommand = new SQLiteCommand(query, this._connection))
                    {
                        if (this._connection.State != ConnectionState.Open)
                            this._connection.Open();

                        createCommand.ExecuteNonQuery();
                    }
                });

                return new Result("", Status.Success);
            }
            catch (Exception ex)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.Append("Error occured on creating the Database Tables");
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.Append(tableName);
                errorBuilder.Append(Environment.NewLine);
                errorBuilder.AppendLine("Exception: " + ex.Message);
                if (ex.InnerException != null)
                    errorBuilder.AppendLine("Inner Exception: " + ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorBuilder.ToString());
                return new Result(errorBuilder.ToString(), Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != ConnectionState.Closed)
                    this._connection.Close();
            }

        }


        /// <summary>
        /// Creates tables if they don't exist yet asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<Result> CreateTablesAsync()
        {
            List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
            int counterForFailure = 0;  // if an error occures, it's easier to find the object which causes the error with a counter
            try
            {
                foreach (CultureInfo culture in cultures)
                {
                    string query = this.GenerateCreateTableQuery(culture);
                    await Task.Run(() =>
                    {
                        using (SQLiteCommand createCommand = new SQLiteCommand(query, this._connection))
                        {
                            if (this._connection.State != ConnectionState.Open)
                                this._connection.Open();

                            createCommand.ExecuteNonQuery();
                        }
                    });

                    counterForFailure++;
                }

                LogWriter.Instance.WriteLine("Database tables created");
                return new Result("", Status.Success);
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
                return new Result(errorBuilder.ToString(), Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != ConnectionState.Closed)
                    this._connection.Close();
            }
        }

        /// <summary>
        /// if directory or file does not exists, it will be created here
        /// </summary>
        public void CreateNewDatabaseFile()
        {
            if (!Directory.Exists(_saveDir))
                Directory.CreateDirectory(_saveDir);

            if (!File.Exists(_savePath))
                SQLiteConnection.CreateFile(_savePath);

            LogWriter.Instance.WriteLine("Database file created");
        }

        #region Vocable
        public Result<int> SaveVocable(Vocable vocable, string tableName)
        {
            if (!Directory.Exists(_saveDir))
                return new Result<int>(null, "Database Directory does not exists", Status.Warning);
            if (!File.Exists(_savePath))
                return new Result<int>(null, "Database File does not exists", Status.Warning);

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(this.GenerateInsertQuery(tableName), this._connection))
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

                    return new Result<int>(-1, "", Status.Success);
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

        public Result UpdateVocable(Vocable vocable, string tableName)
        {
            try
            {

                using (SQLiteCommand command = new SQLiteCommand(this.GenerateUpdateQuery(tableName), this._connection))
                {
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "ID", ParameterName = "@id", Value = vocable.ID });
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
                    return new Result("", Status.Success);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine(String.Format("Error occured at UpdateVocable in DataBaseAccess:\n{0}", ex.Message));
                return new Result(ex.Message, Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != System.Data.ConnectionState.Closed)
                    this._connection.Close();
            }
        }

        public Result DeleteVocable(Vocable vocable, string tableName)
        {
            try
            {

                using (SQLiteCommand command = new SQLiteCommand(this.GenerateDeleteQuery(tableName), this._connection))
                {
                    command.Parameters.Add(new SQLiteParameter() { Command = command, DbType = DbType.String, SourceColumn = "ID", ParameterName = "@id", Value = vocable.ID });

                    if (this._connection.State != ConnectionState.Open)
                        this._connection.Open();

                    command.ExecuteNonQuery();

                    return new Result("", Status.Success);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine(String.Format("Error occured at DeleteVocable in DataBaseAccess:\n{0}", ex.Message));
                return new Result(ex.Message, Status.Error, ex);
            }
            finally
            {
                if (this._connection.State != System.Data.ConnectionState.Closed)
                    this._connection.Close();
            }
        }

        public Result<List<Vocable>> GetAllVocables(string tableName)
        {
            var result = new List<Vocable>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(this.GenerateSelectAllQuery(tableName), this._connection))
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
        #endregion Vocable

        #region Importing
        public  Task<Result> Import(string savePath)
        {
            string importConnectionString = $"Data Source={savePath};Version=3";
            using (var importConnection = new SQLiteConnection(importConnectionString))
            using (var command = new SQLiteCommand(importConnection))
            {
                /*
             TODO: 
             1) get all tables
             2) check which do not exist in local db yet
                2.1) create missing tables    
             3) go through every table and select * from table
             4 - 1) save the selected data temporary until everything is loaded
             4 - 2) select 1 and insert it at once to the local db (requires a new SQLiteConnection to the local DB)
             */
            }
            return null;
            
        }
        #endregion Importing

        #region IDisposable Member
        public void Dispose()
        {
            if (this._connection.State != ConnectionState.Closed)
                this._connection.Close();

            GC.Collect();
        }
        #endregion
    }
}