using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Setting;
using SmartVocabulary.UI;
using System.Speech.Synthesis;

namespace SmartVocabulary
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Actions
        internal Action CloseAction { get; set; }
        internal Action ShowExportWindowAction { get; set; }
        internal Action ShowSettingsWindowAction { get; set; }
        internal Action ShowAboutWindowAction { get; set; }
        internal Action ShowPrintWindowAction { get; set; }
        #endregion Actions

        #region Data
        private readonly VocableLogic _logic;
        internal string ApplicationLocation;
        // this collection if used for searching, to prevent DB access
        // if the search string is empty, the Vocables collection gets over written by this one, which conains all vocables
        private List<Vocable> AllVocablesCollection;
        private SpeechSynthesizer _speechSynthesizer;
        #endregion Data

        #region Properties
        private ObservableCollection<Vocable> _vocables;
        private Vocable _selectedVocable;
        private string _notification;
        private string _alternationRowColor;
        private ObservableCollection<string> _availableLanguages;
        private string _selectedLanguage;
        private bool _areLanguagesAvailable;
        private string _searchText;
        private VocableSearchFilterEnumeration _searchFilter;

        public VocableSearchFilterEnumeration SearchFilter
        {
            get { return _searchFilter; }
            set { SetProperty(ref _searchFilter, value, () => SearchFilter); }
        }
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value, () => SearchText);
                this.Search();
            }
        }
        public bool IsUiEnabled
        {
            get { return this._areLanguagesAvailable; }
            set { this.SetProperty(ref this._areLanguagesAvailable, value, () => this.IsUiEnabled); }
        }
        public string SelectedLanguage
        {
            get { return this._selectedLanguage; }
            set
            {
                this.SetProperty(ref this._selectedLanguage, value, () => this.SelectedLanguage);
                this.EnableControls();
                if (!String.IsNullOrEmpty(this.SelectedLanguage) && this.AvailableLanguages.Contains(this.SelectedLanguage))
                {
                    // Update Settings, set the SelectedLaguage Field
                    SettingsLogic.Instance.UpdateSettings(new Settings()
                    {
                        SelectedLanguage = this.SelectedLanguage
                    });

                    // Load Vocable List for this Language from DB
                    this.LoadVocables();

                    // Initialize Text To Speech culture
                    this.InitializeTextToSpeech();
                }
            }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                this.SetProperty(ref _availableLanguages, value, () => AvailableLanguages);
                EnableControls();
            }
        }
        public string AlternationRowColor
        {
            get { return _alternationRowColor; }
            set { this.SetProperty(ref _alternationRowColor, value, () => AlternationRowColor); }
        }
        public string Notification
        {
            get { return _notification; }
            set { this.SetProperty(ref _notification, value, () => Notification); }
        }
        public Vocable SelectedVocable
        {
            get
            {
                return _selectedVocable;
            }
            set
            {
                this.SetProperty(ref _selectedVocable, value, () => SelectedVocable);
            }
        }
        public ObservableCollection<Vocable> Vocables
        {
            get
            {
                return _vocables;
            }
            set
            {
                this.SetProperty(ref _vocables, value, () => Vocables);
            }
        }
        #endregion Properties

        public MainWindowViewModel()
        {
            this.Notification = "Connection to Database...";
            this._logic = new VocableLogic();

            this.Notification = "Loading Settings...";
            this.LoadSettings();

            this.Notification = "Loading Vocables...";
            Result loadingResult = this.LoadVocables();

            this.Notification = loadingResult.Message;
            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.OpenAboutCommand = new DelegateCommand(this.OpenAbout);
            this.RemoveCommand = new DelegateCommand(this.Remove);
            this.EnterCommand = new DelegateCommand(this.Enter);
            this.ClearSearchFilterCommand = new DelegateCommand(this.ClearSearchFilter);
            this.PlayTextToSpeechCommand = new DelegateCommand(this.PlayTextToSpeech);

            this.RibbonRemoveCommand = new DelegateCommand(this.Remove);
            this.RibbonCloseCommand = new DelegateCommand(this.Close);
            this.RibbonRestartCommand = new DelegateCommand(this.Restart);
            this.RibbonOpenSettingsCommand = new DelegateCommand(this.OpenSettings);
            this.RibbonRefreshCommand = new DelegateCommand(this.RibbonRefresh);
            this.RibbonAddNewCommand = new DelegateCommand(this.RibbonAddNew);
            this.RibbonEditCommand = new DelegateCommand(this.RibbonEdit);
            this.RibbonOpenPrintCommand = new DelegateCommand(this.RibbonOpenPrint);

            this.ExportCommand = new DelegateCommand(this.Export);
            this.ImportCommand = new DelegateCommand(this.Import);
        }

        public ICommand OpenAboutCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand EnterCommand { get; set; }
        public ICommand ClearSearchFilterCommand { get; set; }
        public ICommand PlayTextToSpeechCommand { get; set; }

        public ICommand RibbonCloseCommand { get; set; }
        public ICommand RibbonRestartCommand { get; set; }
        public ICommand RibbonOpenSettingsCommand { get; set; }
        public ICommand RibbonAddNewCommand { get; set; }
        public ICommand RibbonEditCommand { get; set; }
        public ICommand RibbonRemoveCommand { get; set; }
        public ICommand RibbonRefreshCommand { get; set; }

        public ICommand RibbonOpenPrintCommand { get; set; }

        public ICommand ExportCommand { get; set; }
        public ICommand ImportCommand { get; set; }

        private void Import()
        {

        }

        private void Export()
        {
            //ExportWizardWindow window = new ExportWizardWindow();
            //window.Show();
            this.ShowExportWindowAction.Invoke();
        }

        private void ClearSearchFilter()
        {
            this.SearchText = string.Empty;
        }

        private void Enter()
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

        private void RibbonRefresh()
        {
            this.LoadVocables();
        }

        private void RibbonEdit()
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, this.SelectedLanguage, this, this.SelectedVocable);
            editWindow.Show();
        }

        private void RibbonAddNew()
        {
            var editWindow = new EntryDetailWindow();
            editWindow.Initialize(this._logic, this.SelectedLanguage, this);
            editWindow.Show();
        }

        private void RibbonOpenPrint()
        {
            ShowPrintWindowAction.Invoke();
        }

        private void Remove()
        {
            if (this.Vocables.Contains(this.SelectedVocable))
            {
                Result deleteResult = this._logic.DeleteVocable(this.SelectedVocable, this.SelectedLanguage);

                if (deleteResult.Status == Status.Success)
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

        private void OpenAbout()
        {
            //var about = new AboutWindow();
            //about.ShowDialog();
            ShowAboutWindowAction.Invoke();
        }

        private void Close()
        {
            this.CloseAction.Invoke();
        }

        private void Restart()
        {
            Process.Start(this.ApplicationLocation);
            this.CloseAction.Invoke();
        }

        private void OpenSettings()
        {
            this.ShowSettingsWindowAction.Invoke();
            this.LoadSettings();
        }

        private void PlayTextToSpeech()
        {
            if (SelectedVocable == null)
                return;

            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Native);
            Task.Delay(120);
            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Translation);
            Task.Delay(120);
            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Definition);
            Task.Delay(120);
            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Synonym);
            Task.Delay(120);
            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Opposite);
            Task.Delay(120);
            _speechSynthesizer.SpeakAsync(this.SelectedVocable.Example);

        }
        #endregion

        #region Private Methods
        private Result LoadVocables()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Validate if everything is ready for loading
                Result validation = this.ValidateBeforeLoadingVoc();
                if (validation.Status == Status.Warning)
                {
                    return new Result(validation.Message, Status.Warning);
                }

                // Get Data
                Result<List<Vocable>> result = this._logic.GetAllVocables(this.SelectedLanguage);
                if (result.Status != Status.Success)
                {
                    const string notification = "Error on loading Vocables from Database. Please try to refresh, or restart the application. More information can be found in the Log Files";
                    return new Result(notification, Status.Error);
                }

                this.Vocables = new ObservableCollection<Vocable>(result.Data);

                // this is the oly place, where this List is filled
                this.AllVocablesCollection = new List<Vocable>(this.Vocables.ToList());
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            return new Result("Ready", Status.Success);
        }

        private Result ValidateBeforeLoadingVoc()
        {
            // check if DB exists
            Result dbExistence = this.CheckDatabaseExistence();
            if (dbExistence.Status == Status.Warning)
            {
                this.Vocables = new ObservableCollection<Vocable>();
                return new Result(String.Format("{0}. Create new DB in Settings", dbExistence.Message), Status.Warning);
            }

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
                    return new Result("No languages available.", Status.Warning);
                }
            }

            // everything is alright:
            return new Result("", Status.Success);
        }

        private Result CheckDatabaseExistence()
        {
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string savePath = String.Format("{0}\\{1}", saveDir, "smartVocDb.sqlite");

            if (!Directory.Exists(saveDir))
            {
                return new Result("Database Directory does not exist", Status.Warning);
            }
            return !File.Exists(savePath)
                ? new Result("Database file does not exist", Status.Warning)
                : new Result("", Status.Success);
        }

        private void LoadSettings()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Result<Settings> load = SettingsLogic.Instance.LoadSettings();
                if (load.Status == Status.Success)
                {
                    this.AlternationRowColor = load.Data.AlternationColor;
                    this.AvailableLanguages = new ObservableCollection<string>(load.Data.AddedLanguages);
                    this.SelectedLanguage = load.Data.SelectedLanguage;
                    InitializeTextToSpeech();
                }
                else
                {
                    this.AlternationRowColor = "#DFEACE";
                    this.AvailableLanguages = new ObservableCollection<string>();
                }

            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void EnableControls()
        {
            if (this.AvailableLanguages != null
                && this.AvailableLanguages.Count != 0
                && this.SelectedLanguage != null
                && this.AvailableLanguages.Contains(this.SelectedLanguage))
                this.IsUiEnabled = true;
            else
                this.IsUiEnabled = false;
        }

        private void AddNew()
        {
            Result<int> saveResult = this._logic.SaveVocable(this.Vocables.LastOrDefault(), this.SelectedLanguage);
            if (saveResult.Status == Status.Warning)
            {
                // SaveVocable in DBaccess only returns a warning if the DB does not exist
                this.Notification = "Couldn't save the entry. Check if Database exists or create a new one in the Settings";
                this.Vocables.RemoveAt(this.Vocables.IndexOf(this.Vocables.Last()));

                this.Vocables.Add(new Vocable());
                return;
            }

            if (saveResult.Status != Status.Success)
            {
                this.Notification = "Couldn't save the entry. Check LogFiles for more information";
                this.Vocables.RemoveAt(this.Vocables.IndexOf(this.Vocables.Last()));
                return;
            }

            this.Notification = "Entry saved";

            this.SelectedVocable = null;
            this.RibbonRefresh();
            this.Vocables.Add(new Vocable());
            this.Vocables.OrderBy(o => o.ID);
        }

        private void Edit()
        {
            Result updateResult = this._logic.UpdateVocable(this.SelectedVocable, this.SelectedLanguage);
            if (updateResult.Status != Status.Success)
            {
                this.Notification = "Couldn't update the entry. Check LogFiles for more information";
                return;
            }

            this.Notification = "Update successful";
            this.SelectedVocable = null;
        }

        private async void Search()
        {
            if (!String.IsNullOrEmpty(SearchText))
            {
                switch (SearchFilter)
                {
                    case VocableSearchFilterEnumeration.ID:
                        await Task.Run(() =>
                        {
                            this.Vocables = new ObservableCollection<Vocable>(
                                this.AllVocablesCollection
                                    .Where(w => w.ID
                                        .ToString()
                                        .ToLower()
                                        .StartsWith(SearchText.ToLower()))
                                        .ToList());
                        });
                        break;

                    case VocableSearchFilterEnumeration.Kind:
                        await Task.Run(() =>
                        {

                            this.Vocables = new ObservableCollection<Vocable>(
                                this.AllVocablesCollection
                                    .Where(w => w.Kind
                                        .ToString()
                                        .ToLower()
                                        .StartsWith(SearchText.ToLower()))
                                        .ToList());
                        });
                        break;

                    case VocableSearchFilterEnumeration.Native:
                        await Task.Run(() =>
                        {

                            this.Vocables = new ObservableCollection<Vocable>(
                                this.AllVocablesCollection
                                    .Where(w => w.Native
                                        .ToString()
                                        .ToLower()
                                        .StartsWith(SearchText.ToLower()))
                                        .ToList());
                        });
                        break;

                    case VocableSearchFilterEnumeration.Translation:
                        await Task.Run(() =>
                        {

                            this.Vocables = new ObservableCollection<Vocable>(
                                this.AllVocablesCollection
                                    .Where(w => w.Translation
                                        .ToString()
                                        .ToLower()
                                        .StartsWith(SearchText.ToLower()))
                                        .ToList());
                        });
                        break;

                    default:
                        this.Vocables = new ObservableCollection<Vocable>(this.AllVocablesCollection);
                        return;
                }
            }
            else
            {
                this.Vocables = new ObservableCollection<Vocable>(this.AllVocablesCollection);
            }
        }

        private void InitializeTextToSpeech()
        {
            Result<Settings> settings = SettingsLogic.Instance.LoadSettings();
           
            _speechSynthesizer = new SpeechSynthesizer();
            _speechSynthesizer.Rate = 1;
            _speechSynthesizer.Volume = 100;

            var culture = CultureHandler.ConvertStringToCultureInfo(this.SelectedLanguage);
            _speechSynthesizer.SelectVoiceByHints(settings.Data.VoiceGender, settings.Data.VoiceAge, 0, culture);
        }
        #endregion Private Methods
    }
}