using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Data;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Database
{
    /// <summary>
    /// Logic Layer. Work with DatabaseAccess class to get access to the Database
    /// </summary>
    public class VocableLogic : IDisposable
    {
        private readonly DatabaseAccess _access;

        public VocableLogic()
        {
            this._access = new DatabaseAccess();
        }

        public Result<List<Vocable>> GetAllVocables(string language)
        {
            Result<List<Vocable>> result = this._access.GetAllVocables(language);
            if(result.Status != Status.Success)
            {
                string log = String.Format("Error occured in Method: \"GetAllVocables\" in class \"VocableLogic\". Error message:{0}{1}", Environment.NewLine, result.Message);
                LogWriter.Instance.WriteLine(log);
            }
            return result;
        }

        public Result<int> SaveVocable(Vocable entry, string language)
        {
            Result<int> saveResult = this._access.SaveVocable(entry, language);
            if (saveResult.Status != Status.Success)
            {
                string log = String.Format("Error occured in Method: \"SaveVocable\" in class \"VocableLogic\". Error message:{0}{1}", Environment.NewLine, saveResult.Message);
                LogWriter.Instance.WriteLine(log);
            }

            return saveResult;
        }

        public Result UpdateVocable(Vocable entry, string language)
        {
            Result saveResult = this._access.UpdateVocable(entry, language);
            if (saveResult.Status != Status.Success)
            {
                string log = String.Format("Error occured in Method: \"UpdateVocable\" in class \"VocableLogic\". Error message:{0}{1}", Environment.NewLine, saveResult.Message);
                LogWriter.Instance.WriteLine(log);
            }

            return saveResult;
        }

        public async Task<Result<List<Vocable>>> GetAllVocablesAsync(string language)
        {
            return await Task.Run(() => this.GetAllVocables(language));
        }

        public Result DeleteVocable(Vocable entry, string language)
        {
            Result deleteResult = this._access.DeleteVocable(entry, language);
            if(deleteResult.Status != Status.Success)
            {
                string log = String.Format("Error occured in Method: \"DeleteVocable\" in class \"VocableLogic\". Error message:{0}{1}", Environment.NewLine, deleteResult.Message);
                LogWriter.Instance.WriteLine(log);
            }

            return deleteResult;
        }

        #region IDisposable Member

        public void Dispose()
        {
            this._access.Dispose();
        }

        #endregion
    }
}