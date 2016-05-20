using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartVocabulary.Common;
using MessageBox = System.Windows.MessageBox;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// Interaktionslogik für ExportWizardWindow.xaml
    /// </summary>
    public partial class ExportWizardWindow : Window
    {
        public ExportWizardWindow()
        {
            InitializeComponent();
            var viewModel = new ExportWizardWindowViewModel();

            viewModel.CloseAction = new Action(this.Close);
            viewModel.ShowMessageBox = new Func<string, string, string, string, string>(this.ShowMessageBox);
            viewModel.ShowFolderBrowseDialogAction = new Func<bool, string>(this.ShowFolderBrowseDialog);
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Method to be invocated through a Func from ViewModel. Parameter is just a placeholder
        /// </summary>
        /// <param name="param">placeholder, without any use</param>
        /// <returns>the selected direction</returns>
        private string ShowFolderBrowseDialog(bool param)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            return dialog.SelectedPath ?? null;
        }

        private string ShowMessageBox(string messageBoxText, string caption = "", string buttons = "OK", string image = "None")
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

            var result = MessageBox.Show(messageBoxText, caption, button, icon);
            return result.ToString();           
        }
    }
}