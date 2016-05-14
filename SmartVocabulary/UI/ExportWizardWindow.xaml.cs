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
        public ExportWizardWindow()
        {
            InitializeComponent();
            var viewModel = new ExportWizardWindowViewModel();

            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
        }       
    }
}
