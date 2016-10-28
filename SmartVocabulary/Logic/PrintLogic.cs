using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using System.Drawing;

namespace SmartVocabulary.Logic
{
    public class PrintLogic
    {
        public List<string> GetPrinterCollection()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToList();
        }

        public async Task<Result> PrintAsync(string selectedPrinter, List<VocableLanguageWrapper> printItems)
        //public async Task<Result> /*PrintAsync*/(string selectedPrinter, VocableLanguageWrapper printItems)
        {
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
            e.Graphics.DrawLine(new System.Drawing.Pen(Brushes.Black, 1), new System.Drawing.Point(0, 0), new System.Drawing.Point(100, 100));
            e.Graphics.DrawString("Ich kann jetzt mit C# drucken", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new System.Drawing.Point(45, 45));
        }
    }
}
