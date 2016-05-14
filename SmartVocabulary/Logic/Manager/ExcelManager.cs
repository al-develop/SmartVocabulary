using System;
using System.Collections.Generic;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using SpreadsheetLight;
using SpreadsheetLight.Drawing;

namespace SmartVocabulary.Logic.Manager
{
    public class ExcelManager : IManager
    {
        #region IManager Member

        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            using (SLDocument document = new SLDocument(savePath))
            {
                foreach (var collectionMember in vocableCollection)
                {
                    document.AddWorksheet(collectionMember.Language);

                    foreach (Vocable currentVocable in collectionMember.Vocables)
                    {
                        //document.SetCellValue("")
                    }
                }

                document.SaveAs(savePath);
            }
            
            return new Result("Export successfull", Status.Success);
        }

        public Result<List<VocableLanguageWrapper>> Import(string sourcePath)
        {
            throw new NotImplementedException();
        }

        #endregion 

        #region Private
        private void InsertIntoCell()
        {

        }
        #endregion
    }
}
