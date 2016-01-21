using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using SpreadsheetLight;

namespace SmartVocabulary.Logic.Manager
{
    public class ExcelManager : IManager
    {
        #region IManager Member
        
        public Result Export(IList<Vocable> VocableCollection, string savePath)
        {
            SLDocument document = new SLDocument(savePath);
            document.AddWorksheet("Vocable Collection");

            throw new NotImplementedException();
        }

        public Result<IList<Vocable>> Import(string sourcePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
