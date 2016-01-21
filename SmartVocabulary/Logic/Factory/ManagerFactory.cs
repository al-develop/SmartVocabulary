using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Manager;

namespace SmartVocabulary.Logic.Factory
{
    public static class ManagerFactory
    {
        public static IManager GetManager(ManagerEnumeration selection)
        {
            switch(selection)
            {
                case ManagerEnumeration.Excel:
                    LogWriter.Instance.WriteLine("Create new Excel Manager");
                    return new ExcelManager();

                case ManagerEnumeration.PDF:
                    LogWriter.Instance.WriteLine("Create new PDF Manager");
                    return new PdfManager();

                case ManagerEnumeration.XML:
                    LogWriter.Instance.WriteLine("Create new XML Manager");
                    return new XmlManager();

                default:
                    LogWriter.Instance.WriteLine("Wrong Manager passed as parameter to factory");
                    return null;
            }
        }
    }
}
