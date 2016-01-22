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
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// Interaktionslogik für EntryDetailWindow.xaml
    /// </summary>
    public partial class EntryDetailWindow : Window
    {
        
        public EntryDetailWindow()
        {
            InitializeComponent();
        }

        public void Initialize(VocableLogic logic, Vocable entry = null)
        {
            EntryDetailViewModel vm = new EntryDetailViewModel(logic, entry);
            this.DataContext = vm;
            if (vm != null)
                vm.CloseAction = new Action(this.Close);
        }
    }
}
