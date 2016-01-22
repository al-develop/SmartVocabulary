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
            viewModel.CloseAction = new Action(this.Close);
            viewModel.ApplicationLocation = Application.ResourceAssembly.Location;
        }
    }
}