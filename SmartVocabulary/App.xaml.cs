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
            error.AppendLine("" + e.ExceptionObject.GetType().GetMember("Message").ToString());
            LogWriter.Instance.WriteLine(error.ToString());

            //string crashPath = String.Format("{0}//LOGS//CrashLog {1} .txt", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToShortDateString());
            //StringBuilder builder = new StringBuilder();
            //builder.Append(String.Concat(DateTime.Now.ToString("g"), ":\t"));
            //builder.Append(m);

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(crashPath, true))
            //{
            //    file.WriteLine(builder.ToString());
            //}

            //string error = String.Format("An Error occured: {0}. More Information in \"{1}\"", e.GetType().ToString(), crashPath);
            //MessageBox.Show(error, e.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);

            Application.Current.Shutdown();
        }
    }
}
