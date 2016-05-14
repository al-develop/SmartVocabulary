using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartVocabulary.Common;

namespace SmartVocabulary.UI.ExportSettingsPages
{
    /// <summary>
    /// Interaction logic for ExportOverviewPage.xaml
    /// </summary>
    public partial class ExportOverviewPage : UserControl
    {
        public ExportOverviewPage()
        {
            InitializeComponent();

            var viewmodel = new ExportOverviewPageViewModel();
            viewmodel.ShowMessageBox = new Action<string, string, string, string>(this.ShowMessageBox);
        }

        private void ShowMessageBox(string messageBoxText, string caption = "", string buttons = "OK", string image = "None")
        {
            MessageBoxButton button;
            MessageBoxImage icon;
            bool buttonParseSuccess = Enum.TryParse(buttons, out button);
            bool iconParseSuccess = Enum.TryParse(image, out icon);

            if(buttonParseSuccess != true)
            {
                string errorMessage = String.Format("Error occured on calling Method \"ShowMessageBox\" in class \"ExportWizardWindow\". Wrong param was passed: \"buttons\": {0}", buttons);
                LogWriter.Instance.WriteLine(errorMessage);
                throw new InvalidOperationException("ShowMessageBox Action took wrong param: buttons");
            }

            if(iconParseSuccess != true)
            {
                string errorMessage = String.Format("Error occured on calling Method \"ShowMessageBox\" in class \"ExportWizardWindow\". Wrong param was passed: \"image\": {0}", buttons);
                LogWriter.Instance.WriteLine(errorMessage);
                throw new InvalidOperationException("ShowMessageBox Action took wrong param: image");
            }

            MessageBox.Show(messageBoxText, caption, button, icon);
        }
    }
}
