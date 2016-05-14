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
        
        private ObservableCollection<ViewModelBase> _settingsPages;

        public ObservableCollection<ViewModelBase> SettingsPages
        {
            get { return _settingsPages; }
            set { SetProperty(ref _settingsPages, value, () => SettingsPages); }
        }

        public ExportWizardWindowViewModel()
        {
            SettingsPages = new ObservableCollection<ViewModelBase>();
            this.SettingsPages.Add(new ExportOverviewPageViewModel());

            CancelCommand = new DelegateCommand(Cancel);
        }

        public ICommand CancelCommand { get; set; }

        private void Cancel()
        {
            this.CloseAction.Invoke();
        }

    }
}