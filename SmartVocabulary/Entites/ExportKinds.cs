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
        [Description("XML")]
        XML,
        
        [Description("MS Excel")]
        MsExcel,
        
        [Description("Open Office/Libre Office")]
        ApacheOffice,

        [Description("Text (UTF-8, separated by ';' )")]
        Text,

        [Description("PDF")]
        PDF
    }
}