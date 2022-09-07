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
using System.IO;

namespace SmartVocabulary.Logic
{
    public class PrintLogic
    {
        public List<string> GetPrinterCollection() => PrinterSettings.InstalledPrinters.Cast<string>().ToList();

        public async Task<Result> PrintAsync(string selectedPrinter, List<VocableLanguageWrapper> printItems)
        {
            throw new NotImplementedException("MigraDoc not supported anymore. Therefore no new implementation of printing yet.");
            try
            {
                foreach (var voc in printItems)
                {
                    string tempPath = System.IO.Path.GetTempPath() + $"{voc}_smartvoctmp.pdf";

                    var manager = Factory.ManagerFactory.GetManager(ExportKinds.PDF);
                    manager.Export(printItems, tempPath);

                    await Task.Run(() =>
                    {
                        
                        //var doc = new Printer();
                        ////doc.Renderer = new DocumentRenderer(document);
                        //doc.Renderer.PrepareDocument();
                        //doc.Print();
                    });

                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }

                return new Result("Print Success", Status.Success);
            }
            catch (Exception ex)
            {
                LogWriter.Instance.WriteLine($"Error in Printing: {Environment.NewLine}{ex.Message}{Environment.NewLine}class:PrintLogic, Method:Print");
                return new Result($"Error in Printing: {ex.Message}", Status.Error);
            }
            //finally
            //{
            //    if (File.Exists(tempPath))
            //        File.Delete(tempPath);
            //}
        }
    }
}