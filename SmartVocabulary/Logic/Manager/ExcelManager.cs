using System;
using System.Collections.Generic;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using SpreadsheetLight;
using System.IO;

namespace SmartVocabulary.Logic.Manager
{
    public class ExcelManager : IManager
    {
        #region IManager Member
        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            using(var stream = new FileStream(savePath, FileMode.OpenOrCreate))
            using(SLDocument document = new SLDocument(stream))
            {
                foreach(VocableLanguageWrapper collectionMember in vocableCollection)
                {
                    document.AddWorksheet(collectionMember.Language);

                    document.SetCellValue("A1", "ID");
                    document.SetCellValue("B1", "NATIVE");
                    document.SetCellValue("C1", "TRANSLATION");
                    document.SetCellValue("D1", "DEFINITION");
                    document.SetCellValue("E1", "KIND");
                    document.SetCellValue("F1", "SYNONYM");
                    document.SetCellValue("G1", "OPPOSITE");
                    document.SetCellValue("H1", "EXAMPLE");

                    int row = 2;
                    foreach(Vocable currentVocable in collectionMember.Vocables)
                    {
                        document.SetCellValue("A" + row, currentVocable.ID);
                        document.SetCellValue("B" + row, currentVocable.Native);
                        document.SetCellValue("C" + row, currentVocable.Translation);
                        document.SetCellValue("D" + row, currentVocable.Definition);
                        document.SetCellValue("E" + row, currentVocable.Kind.ToString());
                        document.SetCellValue("F" + row, currentVocable.Synonym);
                        document.SetCellValue("G" + row, currentVocable.Opposite);
                        document.SetCellValue("H" + row, currentVocable.Example);
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
    }
}