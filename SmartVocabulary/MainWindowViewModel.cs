using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private bool _areLanguagesAvailable;
        public bool IsUiEnabled
        {
            get { return _areLanguagesAvailable; }
            set { NotifyPropertyChanged(ref _areLanguagesAvailable, value, () => IsUiEnabled); }
        }
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                NotifyPropertyChanged(ref _selectedLanguage, value, () => SelectedLanguage);
                EnableControls();
                if (!String.IsNullOrEmpty(this.SelectedLanguage) && this.AvailableLanguages.Contains(this.SelectedLanguage))
                {
                    // Update Settings, set the SelectedLaguage Field
                    this._settingsManager.UpdateSettings(new Settings()
                    {
                        SelectedLanguage = this.SelectedLanguage
                    });

                    // Load Vocable List for this Language from DB
                    this.LoadVocables();
                }
            }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                NotifyPropertyChanged(ref _availableLanguages, value, () => AvailableLanguages);
                EnableControls();
            }
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
            this.Notification = "Connection to Database...";
            this._logic = new VocableLogic();

            this.Notification = "Loading Vocables...";
            this.LoadVocables();

            this.Notification = "Loading Settings...";
            this._settingsManager = new XmlManager();
            this.LoadSettings();

            this.CommandRegistration();

            this.Notification = "Ready";
        }

        #region Commands
        private void CommandRegistration()
        {
            this.OpenAboutCommand = new BaseCommand(this.OpenAbout);
            this.RemoveCommand = new BaseCommand(this.Remove);
            this.EnterCommand = new BaseCommand(this.Enter);

            this.RibbonRemoveCommand = new BaseCommand(this.Remove);
            this.RibbonCloseCommand = new BaseCommand(this.Close);
            this.RibbonRestartCommand = new BaseCommand(this.Restart);
            this.RibbonOpenSettingsCommand = new BaseCommand(this.OpenSettings);
            this.RibbonRefreshCommand = new BaseCommand(this.RibbonRefresh);
            this.RibbonAddNewCommand = new BaseCommand(this.RibbonAddNew);
            this.RibbonEditCommand = new BaseCommand(this.RibbonEdit);
        }

        public ICommand OpenAboutCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand EnterCommand { get; set; }
        //public ICommand AddNewCommand { get; set; }

        public ICommand RibbonCloseCommand { get; set; }
        public ICommand RibbonRestartCommand { get; set; }
        public ICommand RibbonOpenSettingsCommand { get; set; }
        public ICommand RibbonAddNewCommand { get; set; }
        public ICommand RibbonEditCommand { get; set; }
        public ICommand RibbonRemoveCommand { get; set; }
        public ICommand RibbonRefreshCommand { get; set; }

        private void Enter(object param)
        {
            if (this.SelectedVocable != null && this.SelectedVocable.ID != 0)
            {
                // edit
                this.Edit();
            }
            else if (this.Vocables.LastOrDefault() != null)
            {
                // new entry
                this.AddNew();
            }
        }

        private void RibbonRefresh(object param)
        {
            this.LoadVocables();
        }

        private void RibbonEdit(object param)
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, this.SelectedLanguage, this, this.SelectedVocable);
            editWindow.Show();
        }

        private void RibbonAddNew(object param)
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, this.SelectedLanguage, this, null);
            editWindow.Show();
        }

        private void Remove(object param)
        {
            if(this.Vocables.Contains(this.SelectedVocable))
            {
                Result deleteResult = this._logic.DeleteVocable(this.SelectedVocable, this.SelectedLanguage);

                if(deleteResult.Status == Status.Success)
                {
                    LogWriter.Instance.WriteLine("MainWindowViewModel: Deleting Vocable from DB successful");
                    this.Vocables.Remove(this.SelectedVocable);
                    this.Notification = "Removing Vocable successful";
                }
                else
                {
                    LogWriter.Instance.WriteLine("MainWindowViewModel: Deleting Vocable from DB failed");
                    this.Notification = "Removing Vocable failed. Check Log Files for more information.";
                }
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
        }

        private void OpenSettings(object param)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
            this.LoadSettings();
        }
        #endregion

        private bool LoadVocables()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // check if a language is selected
                if (String.IsNullOrEmpty(this.SelectedLanguage))
                {
                    // check if there are any langauges available
                    if (this.AvailableLanguages != null && this.AvailableLanguages.Count != 0)
                    {
                        this.SelectedLanguage = this.AvailableLanguages.First();
                    }
                    else
                    {
                        this.Vocables = new ObservableCollection<Vocable>();
                        return true;
                    }
                }

                //var result = await this._logic.GetAllVocablesAsync(this.SelectedLanguage);
                var result = this._logic.GetAllVocables(this.SelectedLanguage);
                if (result.Status != Status.Success)
                {
                    this.Notification = "Error on loading Vocables from Database. Please try to refresh, or restart the application. More information can be found in the Log Files";
                    return false;
                }

                this.Vocables = new ObservableCollection<Vocable>(result.Data);
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            return true;
        }

        private async void LoadSettings()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var load = await this._settingsManager.LoadSettingsAsync();
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
            finally { Mouse.OverrideCursor = Cursors.Arrow; }
        }

        private void EnableControls()
        {
            if (AvailableLanguages != null
                && AvailableLanguages.Count != 0
                && this.SelectedLanguage != null
                && AvailableLanguages.Contains(SelectedLanguage))
                IsUiEnabled = true;
            else
                IsUiEnabled = false;
        }

        private void AddNew()
        {
            //if (this.Vocables.LastOrDefault() != null)
            //{
            //    Result<int> saveResult = this._logic.SaveVocable(this.Vocables.LastOrDefault(), this.SelectedLanguage);
            //    if (saveResult.Status != Status.Success)
            //    {
            //        this.Notification = "Couldn't save the entry. Check LogFiles for more information";
            //        this.Vocables.RemoveAt
            //            (this.Vocables.IndexOf
            //                (this.Vocables.Last()));
            //    }

            //    this.Notification = "Entry saved";

            //    this.SelectedVocable = null;
            //    this.RibbonRefresh(param);
            //    this.Vocables.Add(new Vocable());
            //    this.Vocables.OrderBy(o => o.ID);
            //}

            Result<int> saveResult = this._logic.SaveVocable(this.Vocables.LastOrDefault(), this.SelectedLanguage);
            if (saveResult.Status != Status.Success)
            {
                this.Notification = "Couldn't save the entry. Check LogFiles for more information";
                this.Vocables.RemoveAt
                    (this.Vocables.IndexOf
                        (this.Vocables.Last()));
                return;
            }

            this.Notification = "Entry saved";

            this.SelectedVocable = null;
            this.RibbonRefresh(null);
            this.Vocables.Add(new Vocable());
            this.Vocables.OrderBy(o => o.ID);
        }

        private void Edit()
        {
            Result updateResult = this._logic.UpdateVocable(this.SelectedVocable, this.SelectedLanguage);
            if(updateResult.Status != Status.Success)
            {
                this.Notification = "Couldn't update the entry. Check LogFiles for more information";
                return;
            }

            this.Notification = "Update successful";
            this.SelectedVocable = null;
        }
    }
}