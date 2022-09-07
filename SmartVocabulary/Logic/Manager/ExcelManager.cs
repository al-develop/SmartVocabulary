using System;
using System.Collections.Generic;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using SpreadsheetLight;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartVocabulary.Logic.Manager
{
    public class ExcelManager : IManager
    {
        #region IManager Member
        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            // don't    check with File.Exists
            // don't    create with File.Create here,
            // because: it locks the file so it won't be possible to save data in it.
            string file = $"{savePath}\\SmartVocabulary_{DateTime.Now.ToShortDateString()}.xlsx";
            using (SLDocument document = new SLDocument())
            {
                foreach (VocableLanguageWrapper collectionMember in vocableCollection)
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

                    document.SetColumnWidth("A", 5);
                    document.SetColumnWidth("B", 20);
                    document.SetColumnWidth("C", 20);
                    document.SetColumnWidth("D", 20);
                    document.SetColumnWidth("E", 10);
                    document.SetColumnWidth("F", 20);
                    document.SetColumnWidth("G", 20);
                    document.SetColumnWidth("H", 40);


                    SLStyle headerStyle = new SLStyle();
                    headerStyle.Border.SetBottomBorder(BorderStyleValues.Thick, System.Drawing.Color.Black);
                    headerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);


                    document.SetCellStyle("A1", headerStyle);
                    document.SetCellStyle("B1", headerStyle);
                    document.SetCellStyle("C1", headerStyle);
                    document.SetCellStyle("D1", headerStyle);
                    document.SetCellStyle("E1", headerStyle);
                    document.SetCellStyle("F1", headerStyle);
                    document.SetCellStyle("G1", headerStyle);
                    document.SetCellStyle("H1", headerStyle);

                    int row = 2;
                    foreach (Vocable currentVocable in collectionMember.Vocables)
                    {
                        document.SetCellValue("A" + row, currentVocable.ID);
                        document.SetCellValue("B" + row, currentVocable.Native);
                        document.SetCellValue("C" + row, currentVocable.Translation);
                        document.SetCellValue("D" + row, currentVocable.Definition);
                        document.SetCellValue("E" + row, currentVocable.Kind.ToString());
                        document.SetCellValue("F" + row, currentVocable.Synonym);
                        document.SetCellValue("G" + row, currentVocable.Opposite);
                        document.SetCellValue("H" + row, currentVocable.Example);
                        row++;
                    }
                }

                document.SaveAs(file);
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