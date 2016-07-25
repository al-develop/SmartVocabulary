using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartVocabulary.UI;

namespace SmartVocabulary
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // ViewModel Initializing
            var viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;

            // Event and UI Initializing
            viewModel.ApplicationLocation = Application.ResourceAssembly.Location;

            // Action Initiliazing
            viewModel.CloseAction = new Action(this.Close);
            viewModel.ShowExportWindowAction = new Action(this.ShowExportWindow);
            viewModel.ShowSettingsWindowAction = new Action(this.ShowSettingsWindow);
            viewModel.ShowAboutWindowAction = new Action(this.ShowAboutWindow);
            viewModel.ShowPrintWindowAction = new Action(this.ShowPrintWindow);
        }

        private void ShowPrintWindow()
        {
            var window = new PrintWizardWindow();
            window.ShowDialog();
        }

        private void ShowExportWindow()
        {
            var window = new ExportWizardWindow();
            window.ShowDialog();
        }

        private void ShowSettingsWindow()
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void ShowAboutWindow()
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }
    }
}