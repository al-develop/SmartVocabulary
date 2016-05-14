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
        
        [Description("Open Office/Libre Office (*.odt)")]
        ApacheOffice,

        [Description("UTF-8 Unicode Text (.txt)")]
        Text,

        [Description("PDF (*.pdf)")]
        PDF
    }
}