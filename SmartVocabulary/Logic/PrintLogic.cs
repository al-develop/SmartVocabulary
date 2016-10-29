using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using System.Drawing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace SmartVocabulary.Logic
{
    public class PrintLogic
    {
        private List<VocableLanguageWrapper> _printItems;
        private Document _document;

        public List<string> GetPrinterCollection() => PrinterSettings.InstalledPrinters.Cast<string>().ToList();

        public async Task<Result> PrintAsync(string selectedPrinter, List<VocableLanguageWrapper> printItems)
        {
            _printItems = printItems;
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1170);
                    pd.PrinterSettings.PrinterName = selectedPrinter;
                    pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
                    pd.Print();
                });

                return new Result("Print Success", Status.Success);
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine($"Error in Printing: {Environment.NewLine}{ex.Message}{Environment.NewLine}class:PrintLogic, Method:Print");
                return new Result($"Error in Printing: {ex.Message}", Status.Error);
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            //e.Graphics.DrawString("Ich kann jetzt mit C# drucken", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new System.Drawing.Point(45, 45));
        }

        private Result CreateDocument(List<VocableLanguageWrapper> printItems)
        {
            Document document = new Document();
            document.Info.Title = $"SmartVocabulary_{DateTime.Now.ToShortDateString()}";

            this.DefineStyle(document);
            this.DefineContentSection(document);
            //Tuple<Table, Column> tableContent = this.DefineTables(document);
            //// to keep the code easier to read, there's the definition of Table and Column here, so it's easier to read when the Items of the Tuple are used
            //Table table = tableContent.Item1;
            //Column column = tableContent.Item2;

            foreach (var toPrint in printItems)
            {
                // Create Big header with Language name
                Paragraph paragraph = document.LastSection.AddParagraph(toPrint.Language, "Heading1");

                // create table for the current language
                Tuple<Table, Column> tableContent = this.DefineTables(document, toPrint.Vocables);
                // to keep the code easier to read, there's the definition of Table and Column here, so it's easier to read when the Items of the Tuple are used
                //Table table = tableContent.Item1;
                //Column column = tableContent.Item2;

                //this.FillContent(table, column, toPrint.Vocables);
            }

           

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save($"SmartVocabulary_{DateTime.Now.ToShortDateString()}");

            return new Result("", Status.Success);
        }

        /*
         TODO:
         mit einer foreach sollen die vokabel durchgegangen werden und per zugriff auf die entsprechenden Cell values gesetzt werden.
             */
        private Tuple<Table, Column> DefineTables(Document document, List<Vocable> vocables)
        {
            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            cell.AddParagraph("ID");

            cell = row.Cells[1];
            cell.AddParagraph("Native");

            cell = row.Cells[2];
            cell.AddParagraph("Translation");

            cell = row.Cells[3];
            cell.AddParagraph("Definition ");

            cell = row.Cells[4];
            cell.AddParagraph("Kind");

            cell = row.Cells[5];
            cell.AddParagraph("Synonym");

            cell = row.Cells[6];
            cell.AddParagraph("Opposite");

            cell = row.Cells[7];
            cell.AddParagraph("Example");

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
            document.LastSection.Add(table);

            return new Tuple<Table, Column>(table, column);
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
            //style.Font.Name = "Arial";
            //style = document.Styles[StyleNames.Header];
            //style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            //style = document.Styles[StyleNames.Footer];
            //style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Arial";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            //style = document.Styles.AddStyle("Reference", "Normal");
            //style.ParagraphFormat.SpaceBefore = "5mm";
            //style.ParagraphFormat.SpaceAfter = "5mm";
            //style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            return document;
        }

        private void FillContent(Table table, Column column, List<Vocable> vocables)
        {
            int index = 0;
            foreach (var vocable in vocables)
            {
                if (index > vocables.Count())
                    return;

                

                index++;
            }
        }
    }
}