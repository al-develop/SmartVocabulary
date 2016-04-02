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

                    var styles = GenerateExcelStyles();
                    foreach (Vocable currentVocable in collectionMember.Vocables)
                    {

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

        private IList<StyleContainer> GenerateExcelStyles()
        {
            List<StyleContainer> container = new List<StyleContainer>();
            
            SLStyle titleStyle = new SLStyle();
            titleStyle.Font = new SLFont() {Bold = true, FontSize = 25 };


            SLStyle titleBorderStyle = new SLStyle();
            titleBorderStyle.Border = new SLBorder();
            //titleBorderStyle.Border.SetBottomBorder(null, )
           
            /*
             * In der Excel soll auf jeder Seite die Sprache als Überschrift stehen
             * Darunter sind von den Feldern A1 bis H1 die jeweiligen Properties auf des Vocable Klasse als Spalten Namen
             * Zur Übersicht sollte eine Alternation Color eingefügt werden
             */

            return container;
        }

        private class StyleContainer
        {
            public string StyleName { get; set; }
            public SLStyle Style { get; set; }
        }

        #endregion
    }
}
