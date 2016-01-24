using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Data;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Database
{
    /// <summary>
    /// Logic Layer. Work with DatabaseAccess class to get access to the Database
    /// </summary>
    public class VocableLogic
    {
        private readonly DatabaseAccess _access;

        public VocableLogic()
        {
            _access = new DatabaseAccess();
        }

        public Result<IList<Vocable>> GetAllVocables()
        {
            LogWriter.Instance.WriteLine("Excecute Method: \"GetAllVocables\" from class \"VocableLogic\"");

            return null;
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
    }
}
