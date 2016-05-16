using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Data;

namespace SmartVocabulary.Logic.Database
{
    /// <summary>
    /// Class for Controlling Database Work. 
    /// </summary>
    public class DatabaseLogic : IDisposable
    {
        private readonly DatabaseAccess _access;
        private readonly static string _saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private readonly string _savePath = String.Format("{0}\\{1}", _saveDir, "smartVocDb.sqlite");

        public DatabaseLogic()
        {
            _access = new DatabaseAccess();
        }

        public void CreateDatabaseFile()
        {
            this._access.CreateNewDatabaseFile();
        }

        public async Task<Result> CreateTable(string tableName) => await this._access.CreateTableAsync(tableName);

        public async Task<Result> CreateNewDatabaseAsync()
        {
            var createResult = await this._access.CreateNewDatabaseAsync();
            if (createResult.Status != Status.Success)
            {
                StringBuilder log = new StringBuilder();
                log.Append("Error occured in \"DatabaseLogic\". Method: \"CreateNewDatabase\"");
                log.Append(Environment.NewLine);
                log.Append(createResult.Message);
                LogWriter.Instance.WriteLine(log.ToString());
            }

            return createResult;
        }

        [Obsolete]
        public async Task<Result> ResetDatabaseAsync()
        {
            if (Directory.Exists(_saveDir) && File.Exists(_savePath))
            {
                File.Delete(_savePath);
                var createResult = await this.CreateNewDatabaseAsync();
                if (createResult.Status != Status.Success)
                {
                    StringBuilder log = new StringBuilder();
                    log.Append("Error occured in \"DatabaseLogic\". Method: \"ResetDatabaseAsync\"");
                    log.Append(Environment.NewLine);
                    log.Append(createResult.Message);
                    LogWriter.Instance.WriteLine(log.ToString());
                }

                return createResult;
            }

            return new Result("No database found for deleting", Status.Warning);
        }

        public Result DeleteDatabase()
        {
            if (Directory.Exists(_saveDir) && File.Exists(_savePath))
            {
                File.Delete(_savePath);
                return new Result("", Status.Success);
            }

            return new Result("No database found for deleting", Status.Warning);
        }

        #region IDisposable Member
        public void Dispose()
        {
            this._access.Dispose();
        }
        #endregion
    }
}