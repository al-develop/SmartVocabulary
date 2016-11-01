using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Manager;
using SmartVocabulary.Logic.Setting;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// This ViewModel-Class contains Methods and Logic of SettingsWindow.
    /// Properties and Data are in SettingsWindowViewModel.Properties.cs
    /// </summary>
    public partial class SettingsWindowViewModel
    {
        public SettingsWindowViewModel()
        {
            LogWriter.Instance.WriteLine("Constructor of Settings");
            this._databaseLogic = new DatabaseLogic();

            this.Added = new ObservableCollection<string>();
            this.AvailableLanguages = new ObservableCollection<string>();
            this.SettingsAreas = new ObservableCollection<string>();
            this.AreDatabaseOperationsEnabled = true;
            this.IsDatabaseProgressVisible = false;
            this.DatabaseProgress = 0;
            this.GenerateDatabaseProgressMax();
            this.GenerateDatabasePath();

            if (File.Exists(this.DatabasePath))
                this.IsDatabaseExisting = true;
            else
                this.IsDatabaseExisting = false;

            this.SettingAreasRegistration();
            this.CommandRegistration();

            this.SelectedArea = this.SettingsAreas.FirstOrDefault();


            this.LoadCultures();
            this.LoadSettings();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.SaveCommand = new DelegateCommand(this.Save);
            this.AddLanguageCommand = new DelegateCommand(this.AddLanguage);
            this.RemoveLanguageCommand = new DelegateCommand(this.RemoveLanguage);
            this.ClearLanguageSearchCommand = new DelegateCommand(this.ClearLanguageSearch);

            this.CloseCommand = new DelegateCommand(this.Close);

            this.ClearSearchCommand = new DelegateCommand(this.ClearSearch);

            this.CreateNewDatabaseCommand = new DelegateCommand(this.CreateNewDatabase);
            this.ResetDatabaseCommand = new DelegateCommand(this.ResetDatabase);
            this.DeleteDatabaseCommand = new DelegateCommand(this.DeleteDatabase);
            this.ImportDatabaseCommand = new DelegateCommand(this.ImportDatabase);
        }

        // Common
        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

        // Language
        public ICommand AddLanguageCommand { get; set; }
        public ICommand RemoveLanguageCommand { get; set; }
        public ICommand ClearLanguageSearchCommand { get; set; }

        // Database
        public ICommand CreateNewDatabaseCommand { get; set; }
        public ICommand ResetDatabaseCommand { get; set; }
        public ICommand DeleteDatabaseCommand { get; set; }
        public ICommand ImportDatabaseCommand { get; set; }

        #region Common Commands
        private void Close()
        {
            this.CloseAction.Invoke();
        }

        private void Save()
        {
            try
            {
                LogWriter.Instance.WriteLine("Saving Settings begins");

                Settings settings = new Settings()
                {
                    AlternationColor = this.SelectedAlternationColor,
                    AddedLanguages = this.Added.ToList(),
                    VoiceGender = this.SelectedVoiceGender,
                    VoiceAge = this.SelectedVoiceAge
                };

                if (File.Exists(settings.SettingsPath))
                    SettingsLogic.Instance.UpdateSettings(settings);
                else
                    SettingsLogic.Instance.SaveSettings(settings);

                LogWriter.Instance.WriteLine("Saving Settings ends");

            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Error occured in savnig settings - SettingsWindowViewModel;Save()");
                errorMessage.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.AppendLine(ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorMessage.ToString());
            }
            this.CloseAction.Invoke();
        }

        private void ClearSearch()
        {
            this.SearchString = String.Empty;
            this.SettingsAreas.Clear();
            this.SettingAreasRegistration();
        }
        #endregion Common Commands

        #region Language Commands
        private void AddLanguage()
        {
            if (!this.Added.Contains(this.SelectedAvailable))
            {
                this.Added.Add(this.SelectedAvailable);
            }
        }

        private void RemoveLanguage()
        {
            if (this.Added.Contains(this.SelectedAdded))
            {
                this.Added.Remove(this.SelectedAdded);
            }
        }

        private void ClearLanguageSearch()
        {
            this.LanguageSearchText = string.Empty;
            this.AvailableLanguages.Clear();
            this.LoadCultures();
        }
        #endregion Language Commands

        #region Database Commands
        private async void CreateNewDatabase()
        {
            try
            {
                LogWriter.Instance.WriteLine("Creating new DB begins");
                this.IsDatabaseProgressVisible = true;
                this.AreDatabaseOperationsEnabled = false;
                this._databaseLogic.CreateDatabaseFile();
                List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();

                foreach (CultureInfo culture in cultures)
                {
                    var createResult = await this._databaseLogic.CreateTable(culture.NativeName);
                    if (createResult.Status != Status.Success)
                    {
                        MessageBox.Show("Error occured while creating a new database. Check log file for more information", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (this.DatabaseProgress <= this.DatabaseProgressMax)
                    {
                        this.DatabaseProgress++;
                        double percent = (this.DatabaseProgress / (double)this.DatabaseProgressMax) * 100.0;
                        this.DatabaseProgressInPercent = String.Format("{0} %", Math.Round(percent, 0, MidpointRounding.ToEven));
                    }
                }

                for (int i = this.DatabaseProgressMax; i != 0;)
                {
                    this.DatabaseProgress = i;
                    i = i - 10;
                    //await Task.Delay(5);
                }

                this.DatabaseProgressInPercent = "Creating done";
                this.GenerateDatabasePath();
                this.IsDatabaseExisting = true;
            }
            catch (Exception ex)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Error occured in creating new DB");
                errorBuilder.AppendLine("Exception: " + ex.Message);
                if (ex.InnerException != null)
                    errorBuilder.AppendLine("Inner Exception:" + ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorBuilder.ToString());
            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
            }
        }

        private async void ResetDatabase()
        {
            try
            {

                this.IsDatabaseProgressVisible = true;
                this.AreDatabaseOperationsEnabled = false;

                MessageBoxResult result = MessageBox.Show("Are you sure, you want to reset the database? It cannot be undone.", "Reset database", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
                    this._databaseLogic.DeleteDatabase();
                    this._databaseLogic.CreateDatabaseFile();

                    foreach (CultureInfo culture in cultures)
                    {
                        var createResult = await this._databaseLogic.CreateTable(culture.NativeName);
                        if (createResult.Status != Status.Success)
                        {
                            StringBuilder log = new StringBuilder();
                            log.Append("Error occured in \"SettingsViewModel\". Method:\"ResetDatabase\"");
                            log.Append(Environment.NewLine);
                            log.Append(createResult.Message);
                            LogWriter.Instance.WriteLine(log.ToString());

                            MessageBox.Show(log.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (this.DatabaseProgress <= this.DatabaseProgressMax)
                        {
                            this.DatabaseProgress++;
                            int percent = (this.DatabaseProgress / this.DatabaseProgressMax) * 100;
                            this.DatabaseProgressInPercent = String.Format("{0} %", percent.ToString());
                        }
                    }

                    for (int i = this.DatabaseProgressMax; i != 0;)
                    {
                        this.DatabaseProgress = i;
                        i = i - 10;
                        await Task.Delay(5);
                    }

                    this.DatabaseProgressInPercent = "Reset done";
                }
            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
            }
        }

        private void DeleteDatabase()
        {
            try
            {
                this.IsDatabaseProgressVisible = false;
                this.AreDatabaseOperationsEnabled = false;

                MessageBoxResult result = MessageBox.Show("Are you sure, you want to delete the whole database? It cannot be undone.", "Delete database", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Result deleteResult = this._databaseLogic.DeleteDatabase();
                    if (deleteResult.Status != Status.Success)
                    {
                        StringBuilder log = new StringBuilder();
                        log.Append("Error occured in \"SettingsViewModel\". Method:\"DeleteDatabase\"");
                        log.Append(Environment.NewLine);
                        log.Append(deleteResult.Message);
                        LogWriter.Instance.WriteLine(log.ToString());

                        MessageBox.Show(log.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
            }
        }

        private void ImportDatabase()
        {
            string file = SelectImportPathAction.Invoke();
            if (!File.Exists(file))
            {
                this.ShowMessageBoxAction.Invoke("selected file does not exists", "Error");
                LogWriter.Instance.WriteLine($"Error on importing Database: class:SeetingsWindowViewModel{Environment.NewLine}\tmethod:ImportDatabase");
                return;
            }

            try
            {
                this.AreDatabaseOperationsEnabled = false;
                IsImporting = true;


            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
                IsImporting = false;
            }
        }
        #endregion Database Commands

        #endregion Commands

        #region Methods
        private async void LoadSettings()
        {
            Result<Settings> load = await SettingsLogic.Instance.LoadSettingsAsync();
            if (load.Status == Status.Success)
            {
                this.SelectedAlternationColor = load.Data.AlternationColor;
                this.Added = new ObservableCollection<string>(load.Data.AddedLanguages);
                this.SelectedVoiceAge = load.Data.VoiceAge;
                this.SelectedVoiceGender = load.Data.VoiceGender;
            }
        }

        private void LoadCultures()
        {
            LogWriter.Instance.WriteLine("Loading cultures - SettingsWindowViewModel;LoadCultures()");
            try
            {
                List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
                foreach (CultureInfo culture in cultures)
                {
                    this.AvailableLanguages.Add(culture.NativeName);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Error occured in loading cultures - SettingsWindowViewModel;LoadCultures()");
                errorMessage.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.AppendLine(ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorMessage.ToString());
            }
        }

        private void SettingAreasRegistration()
        {
            this.SettingsAreas = new ObservableCollection<string>(ConstantSettingAreas.GetAllSettingAreas());
        }

        private void AreaSelectionChanged()
        {
            if (String.IsNullOrEmpty(SelectedArea))
                return;

            switch (SelectedArea.ToLower())
            {
                case "languages":
                    this.LanguagePageVisibility = true;
                    this.RowPageVisibility = false;
                    this.DatabaseSettingsVisibility = false;
                    this.TextToSpeechVisibility = false;
                    break;

                case "row appearance":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = true;
                    this.DatabaseSettingsVisibility = false;
                    this.TextToSpeechVisibility = false;
                    break;

                case "database settings":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = false;
                    this.DatabaseSettingsVisibility = true;
                    this.TextToSpeechVisibility = false;
                    break;

                case "text to speech":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = false;
                    this.DatabaseSettingsVisibility = false;
                    this.TextToSpeechVisibility = true;
                    break;

                default:
                    return;
            }
        }

        private void GenerateDatabasePath()
        {
            LogWriter.Instance.WriteLine("Generating DB path");
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string savePath = $"{saveDir}\\{"smartVocDb.sqlite"}";

            if (Directory.Exists(saveDir))
            {
                if (File.Exists(savePath))
                {
                    this.DatabasePath = savePath;
                }
            }
        }

        private void GenerateDatabaseProgressMax()
        {
            List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
            this.DatabaseProgressMax = cultures.Count;
        }

        private void FilterLanguages()
        {
            LogWriter.Instance.WriteLine("Filtering Languages - SettingsWindowViewModel;FilterLanguages()");
            try
            {
                this.AvailableLanguages.Clear();
                if (!String.IsNullOrWhiteSpace(this.LanguageSearchText))
                {
                    var temp = CultureHandler.GetCulturesAsLowerCaseStringCollectionByFilter();
                    var filtered = new ObservableCollection<string>(temp.Where(w => w.StartsWith(this.LanguageSearchText.ToLower())));
                    foreach (var f in filtered)
                    {
                        this.AvailableLanguages.Add(f.First().ToString().ToUpper() + String.Join("", f.Skip(1)));
                    }
                }
                else
                {
                    this.LoadCultures();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Error occured in filtering cultures - SettingsWindowViewModel;FilterLanguages()");
                errorMessage.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.AppendLine(ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorMessage.ToString());
            }
        }

        private void FilterSettings()
        {
            this.SettingsAreas.Clear();
            if (!String.IsNullOrEmpty(this.SearchString))
            {
                var temp = ConstantSettingAreas.GetAllSettingAreas().Select(s => s.ToLower());
                var filtered = new ObservableCollection<string>(temp.Where(w => w.StartsWith(this.SearchString.ToLower())));
                foreach (var f in filtered)
                {
                    this.SettingsAreas.Add(f.First().ToString().ToUpper() + String.Join("", f.Skip(1)));
                }
            }
            else
            {
                this.SettingAreasRegistration();
            }
        }
        #endregion Methods
    }
}