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

namespace SmartVocabulary.Data
{
    /// <summary>
    /// Data Layer. Work directly with SQL and the Database
    /// </summary>
    public class DatabaseAccess
    {
        private string _connectionString;
        private static List<Vocable> _preLoadedList;
        private static string _saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private string _savePath = String.Format("{0}\\{1}", _saveDir, "smartVocDb.sqlite");
        private SQLiteConnection _connection;

        public DatabaseAccess()
        {
            if(!Directory.Exists(_saveDir))
                Directory.CreateDirectory(_saveDir);
            if (!File.Exists(_savePath))
                CreateDatabase();

            String.Format("Data Source={0};Version=3", _connectionString);
            _connection = new SQLiteConnection(_connectionString);
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(_savePath);
            this.CreateTable("");
        }

        public Result CreateTable(string tableName)
        {
            return new Result();
        }

        public Result SaveVocable(Vocable vocable)
        {
            


            if (_preLoadedList != null)
            {
                _preLoadedList.Add(vocable);
            }
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
    }
}