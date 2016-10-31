using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Entites
{
    public enum ExportKinds
    {
        [Description("XML (*.xml)")]
        XML,

        [Description("MS Excel (*.xlsx)")]
        MsExcel,

        //[Description("UTF-8 Unicode Text (.txt)")]
        //Text,

        [Description("PDF (*.pdf)")]
        PDF
    }

    public static class ExportKindsExtrahator
    {
        public static string GetExportKindExtension(ExportKinds kind)
        {
            switch(kind)
            {
                case ExportKinds.XML:
                    return ".xml";

                case ExportKinds.MsExcel:
                    return ".xlsx";                    

                //case ExportKinds.Text:
                //    return ".txt";

                case ExportKinds.PDF:
                    return ".pdf";

                default:
                    return null;
            }
        }
    }
}