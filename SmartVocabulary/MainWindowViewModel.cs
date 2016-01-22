using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
using System.Windows.Input;
using BaseMvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Manager;
using SmartVocabulary.UI;

namespace SmartVocabulary
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Data
        private readonly VocableLogic _logic;
        private readonly XmlManager _settingsManager;
        internal string ApplicationLocation;
        internal Action CloseAction { get; set; }
        #endregion Data

        #region Properties
        private ObservableCollection<Vocable> _vocables;
        private Vocable _selectedVocable;
        private string _notification;
        private string _alternationRowColor;
        private ObservableCollection<string> _availableLanguages;
        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                NotifyPropertyChanged(ref _selectedLanguage, value, () => SelectedLanguage);
                if (!String.IsNullOrEmpty(this.SelectedLanguage) && this.AvailableLanguages.Contains(this.SelectedLanguage))
                {
                    this._settingsManager.UpdateSettings(new Settings()
                    {
                        SelectedLanguage = this.SelectedLanguage
                    });
                }
            }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set { NotifyPropertyChanged(ref _availableLanguages, value, () => AvailableLanguages); }
        }
        public string AlternationRowColor
        {
            get { return _alternationRowColor; }
            set { NotifyPropertyChanged(ref _alternationRowColor, value, () => AlternationRowColor); }
        }
        public string Notification
        {
            get { return _notification; }
            set { NotifyPropertyChanged(ref _notification, value, () => Notification); }
        }
        public Vocable SelectedVocable
        {
            get { return _selectedVocable; }
            set { NotifyPropertyChanged(ref _selectedVocable, value, () => SelectedVocable); }
        }
        public ObservableCollection<Vocable> Vocables
        {
            get { return _vocables; }
            set { NotifyPropertyChanged(ref _vocables, value, () => Vocables); }
        }
        #endregion Properties

        public MainWindowViewModel()
        {
            this._logic = new VocableLogic();
            this._settingsManager = new XmlManager();
            this.LoadSettings();
            this.Vocables = new ObservableCollection<Vocable>();
            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.OpenAboutCommand = new BaseCommand(this.OpenAbout);
            this.RemoveCommand = new BaseCommand(this.Remove);
            this.AddNewCommand = new BaseCommand(this.AddNew);

            this.RibbonCloseCommand = new BaseCommand(this.Close);
            this.RibbonRestartCommand = new BaseCommand(this.Restart);
            this.RibbonOpenSettingsCommand = new BaseCommand(this.OpenSettings);
        }

        public ICommand OpenAboutCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        
        public ICommand RibbonCloseCommand { get; set; }
        public ICommand RibbonRestartCommand { get; set; }
        public ICommand RibbonOpenSettingsCommand { get; set; }
        public ICommand RibbonAddNewCommand { get; set; }
        public ICommand RibbonEditCommand { get; set; }
        public ICommand RibbonRemoveCommand { get; set; }

        private void RibbonRemove(object param)
        {
            this.Remove(param);
        }

        private void RibbonEdit(object param)
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, this.SelectedVocable);
            editWindow.Show();
        }

        private void RibbonAddNew(object param)
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, null);
            editWindow.Show();
        }

        private void Remove(object param)
        {

        }

        private void AddNew(object param)
        {

            if (this.Vocables.LastOrDefault() != null && this.Vocables.Last().ID == this.SelectedVocable.ID)
            {
                //this._logic.SaveVocable(this.Vocables.LastOrDefault());
                //this.SelectedVocable = null;
                //this.Vocables = new ObservableCollection<Vocable>();
                this.Vocables.Add(new Vocable());
            }
        }

        private void OpenAbout(object param)
        {
            AboutWindow about = new AboutWindow();
            about.Topmost = true;
            about.ShowDialog();
        }

        private void Close(object param)
        {
            //Application.Current.Shutdown();
            this.CloseAction.Invoke();
        }

        private void Restart(object param)
        {
            Process.Start(this.ApplicationLocation);
            this.CloseAction.Invoke();
            //Application.Current.Shutdown();
        }

        private void OpenSettings(object param)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
            this.LoadSettings();
        }
        #endregion

        private void LoadVocables()
        {
            VocableLogic logic = new VocableLogic();
            var result = logic.GetAllVocables();
            if (result.Status != Status.Success)
            {
                this.Notification = "Error occured on Loading Vocable List. Check Log for more Information";
                return;
            }

            this.Vocables = new ObservableCollection<Vocable>(result.Data);
        }

        private void LoadSettings()
        {
            var load = this._settingsManager.LoadSettings();
            if (load.Status == Common.Status.Success)
            {
                this.AlternationRowColor = load.Data.AlternationColor;
                this.AvailableLanguages = new ObservableCollection<string>(load.Data.AddedLanguages);
                this.SelectedLanguage = load.Data.SelectedLanguage;
            }
            else
            {
                this.AlternationRowColor = "#DFEACE";
                this.AvailableLanguages = new ObservableCollection<string>();
            }
        }
    }
}
