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
using MessageBox = System.Windows.MessageBox;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SettingsWindowViewModel viewmodel;
            viewmodel = new SettingsWindowViewModel();
            this.DataContext = viewmodel;
            if (viewmodel.CloseAction == null)
                viewmodel.CloseAction = new Action(this.Close);

            if (viewmodel.SelectImportPathAction == null)
                viewmodel.SelectImportPathAction = new Func<string>(SelectImportPath);

            if (viewmodel.ShowMessageBoxAction == null)
                viewmodel.ShowMessageBoxAction = new Action<string, string>(ShowMessageBox);
        }

        private string SelectImportPath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "SQLite databases(.sqlite)|*.sqlite";
            dialog.Multiselect = false;

            //if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    return dialog.FileName;
            //}

            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK 
                                        ? null 
                                        : dialog.FileName;
        }

        private void ShowMessageBox(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}