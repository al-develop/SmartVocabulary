using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;

namespace SmartVocabulary.Logic.Manager
{
    public class TextManager: IManager
    {
        #region IManager Member

        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            throw new NotImplementedException();
        }

        public Result<List<VocableLanguageWrapper>> Import(string sourcePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
