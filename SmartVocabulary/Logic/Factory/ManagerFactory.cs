using System;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Manager;

namespace SmartVocabulary.Logic.Factory
{
    public static class ManagerFactory
    {
        public static IManager GetManager(ExportKinds selection)
        {
            switch(selection)
            {
                case ExportKinds.MsExcel:
                    LogWriter.Instance.WriteLine("Create new Excel Manager");
                    return new ExcelManager();
                    
                case ExportKinds.XML:
                    LogWriter.Instance.WriteLine("Create new XML Manager");
                    return new XmlManager();

                case ExportKinds.PDF:
                    LogWriter.Instance.WriteLine("Create new PDF Manager");
                    return new PdfManager();

                //case ExportKinds.Text:
                //    LogWriter.Instance.WriteLine("Create new Text Manager");
                //    return new TextManager();

                //case ExportKinds.ApacheOffice:
                //    throw new NotImplementedException("Open Office and Libre Office export ot implemented yet");

                default:
                    LogWriter.Instance.WriteLine("Wrong Manager passed as parameter to manager factory in export/import");
                    throw new InvalidOperationException("Wrong Manager passed as parameter to manager factory in export/import");
            }
        }
    }
}
