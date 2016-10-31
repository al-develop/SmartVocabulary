using DevExpress.Mvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartVocabulary.UI
{
    public class PrintWizardWindowViewModel : ViewModelBase
    {
        #region Data
        private readonly PrintLogic _logic;
        public Action CloseAction;
        #endregion Data

        #region Properties
        private ObservableCollection<string> _printerList;
        private string _selectedPrinter;
        private ObservableCollection<string> _availableLanguages;
        private ObservableCollection<string> _selectedLanguages;
        private string _selectedLanguage;
        private bool _isLanguageListSelected;

        public bool IsLanguageListSelected
        {
            get { return _isLanguageListSelected; }
            set
            {
                SetProperty(ref _isLanguageListSelected, value, () => IsLanguageListSelected);
                if (IsLanguageListSelected)
                    this.SelectAllLanguagesCommand.Execute(null);
                else
                    this.SelectNoneLanguagesCommand.Execute(null);
            }
        }
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { SetProperty(ref _selectedLanguage, value, () => SelectedLanguage); }
        }
        public ObservableCollection<string> SelectedLanguages
        {
            get { return _selectedLanguages; }
            set { SetProperty(ref _selectedLanguages, value, () => SelectedLanguages); }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set { SetProperty(ref _availableLanguages, value, () => AvailableLanguages); }
        }
        public string SelectedPrinter
        {
            get { return _selectedPrinter; }
            set { SetProperty(ref _selectedPrinter, value, () => SelectedPrinter); }
        }
        public ObservableCollection<string> PrinterList
        {
            get { return _printerList; }
            set { SetProperty(ref _printerList, value, () => PrinterList); }
        }
        #endregion Properties


        public PrintWizardWindowViewModel()
        {
            _logic = new PrintLogic();

            this.PrintCommand = new AsyncCommand(this.Print);
            this.CloseCommand = new DelegateCommand(this.Close);
            this.SelectNoneLanguagesCommand = new DelegateCommand(this.SelectNoneLanguages);
            this.SelectAllLanguagesCommand = new DelegateCommand(this.SelectAllLanguages);

            Result<Settings> settings = SettingsLogic.Instance.LoadSettings();

            this.AvailableLanguages = new ObservableCollection<string>(settings.Data?.AddedLanguages);
            this.PrinterList = new ObservableCollection<string>(this._logic.GetPrinterCollection());
            this.SelectedLanguages = new ObservableCollection<string>();
        }


        public ICommand PrintCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectNoneLanguagesCommand { get; set; }
        public ICommand SelectAllLanguagesCommand { get; set; }

        private void SelectAllLanguages()
        {
            SelectedLanguages.Clear();
            foreach (var lang in AvailableLanguages)
            {
                SelectedLanguages.Add(lang);
            }
        }

        private void SelectNoneLanguages()
        {
            this.SelectedLanguages.Clear();
        }

        private async Task Print()
        {
            List<VocableLanguageWrapper> selecetPrintItems = this.GeneretaPrintItems();

            await _logic.PrintAsync(this.SelectedPrinter, selecetPrintItems);
        }

        private List<VocableLanguageWrapper> GeneretaPrintItems()
        {
            var selectedLanguages = new List<VocableLanguageWrapper>();
            var dataLogic = new VocableLogic();

            foreach (string language in this.SelectedLanguages)
            {
                selectedLanguages.Add(new VocableLanguageWrapper()
                {
                    Language = language,
                    Vocables = dataLogic.GetAllVocables(language).Data
                });
            }

            return selectedLanguages;
        }

        private void Close()
        {
            CloseAction.Invoke();
        }
    }
}
