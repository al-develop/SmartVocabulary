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

        public Result Export(IList<KeyValuePair<string, IList<Vocable>>> vocableCollection, string savePath)
        {
            SLDocument document = new SLDocument(savePath);
            foreach (var collectionMember in vocableCollection)
            {
                document.AddWorksheet(collectionMember.Key);

                CreateExcelDocumentTitle(ref document);
                foreach (Vocable currentVocable in collectionMember.Value)
                {

                }
            }

            document.SaveAs(savePath);
            document.Dispose();
            
            return new Result("Export successfull", Status.Success);
        }

        public Result<IList<Vocable>> Import(string sourcePath)
        {
            throw new NotImplementedException();
        }

        private void CreateExcelDocumentTitle(ref SLDocument document)
        {
            SLStyle titleStyle = new SLStyle();
            /*
             * In der Excel soll auf jeder Seite die Sprache als Überschrift stehen
             * Darunter sind von den Feldern A1 bis H1 die jeweiligen Properties auf des Vocable Klasse als Spalten Namen
             * Zur Übersicht sollte eine Alternation Color eingefügt werden             * 
             */

        }

        #endregion
    }
}
