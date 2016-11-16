using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SmartVocabulary.Common;

namespace SmartVocabulary
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));
            base.OnStartup(e);


            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = new StringBuilder();
            error.AppendLine("An Unhandled Exception occured");
            error.AppendLine("Exception Type: " + e.ExceptionObject.GetType().ToString());
            error.AppendLine("Message:        " + e.ExceptionObject.GetType().GetMember("Message").ToString());
            error.AppendLine("Assembly:       " + e.ExceptionObject.GetType().Assembly);
            error.AppendLine("FullName:       " + e.ExceptionObject.GetType().FullName);
            //error.AppendLine("" + e.ExceptionObject.GetType())
            LogWriter.Instance.WriteLine(error.ToString());
                        
            //Application.Current.Shutdown();
        }
    }
}
