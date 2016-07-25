using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;

namespace SmartVocabulary.Logic.Manager
{
    public class PdfManager : IManager
    {
        #region IManager Member

        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = new PdfPage();

                document.Save(savePath);
            }

            return null;
        }

        public Result<List<VocableLanguageWrapper>> Import(string sourcePath)
        {
            throw new NotImplementedException("PDF Import not supported");
        }

        #endregion
    }
}