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
using System.Windows.Shapes;
using SmartVocabulary.Common;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// Interaktionslogik für ExportWizardWindow.xaml
    /// </summary>
    public partial class ExportWizardWindow : Window
    {
        public ExportWizardWindow(List<string> availableLanguages)
        {
            InitializeComponent();
            var viewModel = new ExportWizardWindowViewModel(availableLanguages);
            viewModel.CloseAction = new Action(this.Close);
            viewModel.ShowMessageBox = new Action<string, string, string, string>(this.ShowMessageBox);

            this.DataContext = viewModel;
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
