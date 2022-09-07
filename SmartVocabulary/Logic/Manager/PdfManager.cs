using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.IO;
using System.Windows.Markup;
using static System.Net.WebRequestMethods;

namespace SmartVocabulary.Logic.Manager
{
    public class PdfManager : IManager
    {
        private const string HEADER_HEADING = "Heading2";
        #region IManager Member

        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            var result = this.CreateDocument(vocableCollection, savePath);
            return result;
        }

        public Result<List<VocableLanguageWrapper>> Import(string sourcePath)
        {
            throw new NotImplementedException("PDF Import not supported");
        }

        #endregion IManager Member

        private Result CreateDocument(List<VocableLanguageWrapper> printItems, string savePath)
        {
            foreach (var toPrint in printItems)
            {
                string language = toPrint.Language;
                string filePath = $"{savePath}\\{language}_{DateTime.Now.ToShortDateString()}.pdf";
                Document document = new Document();
                document.Info.Title = filePath.TrimEnd('.');

                this.DefineStyle(document);
                this.DefineContentSection(document);

                this.CreateHeader(toPrint, document);
                this.CreateTables(document, toPrint.Vocables);

                this.RenderDocument(filePath, document);
            }

            return new Result("", Status.Success);
        }

        private void CreateHeader(VocableLanguageWrapper toPrint, Document document)
        {
            string language = $"{toPrint.Language}";
            string vocableCount = $"({toPrint.Vocables.Count})";
            string paragraphContent = $"{language} {vocableCount}";

            Paragraph paragraph = document.LastSection.AddParagraph(paragraphContent, HEADER_HEADING);
        }

        private void RenderDocument(string file, Document document)
        {
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(file);
        }

        private void CreateTables(Document document, List<Vocable> vocables)
        {
            Table table = new Table();
            table.Borders.Width = 0.75;

            // Create Columns
            //Column columnID = table.AddColumn(Unit.FromCentimeter(2));
            //columnID.Format.Alignment = ParagraphAlignment.Left;
            //columnID.Width = new Unit(1, UnitType.Centimeter);

            Column columnNative = table.AddColumn(Unit.FromCentimeter(2));
            columnNative.Format.Alignment = ParagraphAlignment.Left;
            columnNative.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnTranslation = table.AddColumn(Unit.FromCentimeter(2));
            columnTranslation.Format.Alignment = ParagraphAlignment.Left;
            columnTranslation.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnDefinition = table.AddColumn(Unit.FromCentimeter(2));
            columnDefinition.Format.Alignment = ParagraphAlignment.Left;
            columnDefinition.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnKind = table.AddColumn(Unit.FromCentimeter(2));
            columnKind.Format.Alignment = ParagraphAlignment.Left;
            columnKind.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnSynonym = table.AddColumn(Unit.FromCentimeter(2));
            columnSynonym.Format.Alignment = ParagraphAlignment.Left;
            columnSynonym.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnOpposite = table.AddColumn(Unit.FromCentimeter(2));
            columnOpposite.Format.Alignment = ParagraphAlignment.Left;
            columnOpposite.Width = new Unit(2.3, UnitType.Centimeter);

            Column columnExample = table.AddColumn(Unit.FromCentimeter(2));
            columnExample.Format.Alignment = ParagraphAlignment.Left;
            columnExample.Width = new Unit(2.3, UnitType.Centimeter);

            // Crate Row for Header
            Row headerRow = table.AddRow();

            // Fill Header
            //Cell headerCell = headerRow.Cells[0];
            //headerCell.AddParagraph("ID");

            Cell headerCell = headerRow.Cells[0];
            headerCell.AddParagraph("Native");

            headerCell = headerRow.Cells[1];
            headerCell.AddParagraph("Translation");

            headerCell = headerRow.Cells[2];
            headerCell.AddParagraph("Definition");

            headerCell = headerRow.Cells[3];
            headerCell.AddParagraph("Kind");

            headerCell = headerRow.Cells[4];
            headerCell.AddParagraph("Synonym");

            headerCell = headerRow.Cells[5];
            headerCell.AddParagraph("Opposite");

            headerCell = headerRow.Cells[6];
            headerCell.AddParagraph("Example");

            // Create rows and fill vocabulary content in it
            foreach (var voc in vocables)
            {
                Row row = table.AddRow();

                //Cell cell = row.Cells[0];
                //cell.AddParagraph(voc.ID.ToString());

                Cell cell = row.Cells[0];
                cell.AddParagraph(voc.Native);

                cell = row.Cells[1];
                cell.AddParagraph(voc.Translation);

                cell = row.Cells[2];
                cell.AddParagraph(voc.Definition);

                cell = row.Cells[3];
                cell.AddParagraph(voc.Kind.ToString());

                cell = row.Cells[4];
                cell.AddParagraph(voc.Synonym);

                cell = row.Cells[5];
                cell.AddParagraph(voc.Opposite);

                cell = row.Cells[6];
                cell.AddParagraph(voc.Example);
            }

            //table.SetEdge(0, 0, 0, 0, Edge.Box, BorderStyle.Single, 2, Colors.Black);
            table.SetEdge(0, 0, 0, 0, Edge.Box, BorderStyle.Single, new Unit(6, UnitType.Centimeter), Colors.Black);

            document.LastSection.Add(table);
        }

        private Document DefineContentSection(Document document)
        {
            Section section = document.AddSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;
            return document;
        }

        private Document DefineStyle(Document document)
        {
            Style style = document.Styles["Normal"];

            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Arial";
            style.Font.Size = 5;

            return document;
        }
    }
}