using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;
using SmartVocabulary.UI.ExportSettingsPages;

namespace SmartVocabulary.UI
{
    public class ExportWizardWindowViewModel : ViewModelBase
    {
        public Action CloseAction;
        
        private ObservableCollection<ViewModelBase> _exportSettingsPages;

        public ObservableCollection<ViewModelBase> ExportSettingsPages
        {
            get { return _exportSettingsPages; }
            set { SetProperty(ref _exportSettingsPages, value, () => ExportSettingsPages); }
        }

        public ExportWizardWindowViewModel()
        {
            ExportSettingsPages = new ObservableCollection<ViewModelBase>();
            this.ExportSettingsPages.Add(new ExportOverviewPageViewModel());

            CancelCommand = new DelegateCommand(Cancel);
        }

        public ICommand CancelCommand { get; set; }

        private void Cancel()
        {
            this.CloseAction.Invoke();
        }

    }
}