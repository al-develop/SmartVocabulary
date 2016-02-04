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
    public class PdfManager : IManager
    {
        #region IManager Member


        #endregion
        #region IManager Member

        public Result Export(IList<Vocable> vocableCollection, string savePath)
        {
            throw new NotImplementedException();
        }

        public Result<IList<Vocable>> Import(string sourcePath)
        {
            throw new NotImplementedException("PDF Import not supported");
        }

        #endregion
    }
}