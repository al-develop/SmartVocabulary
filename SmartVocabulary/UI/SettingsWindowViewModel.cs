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
using System.Windows.Media;
using BaseMvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Manager;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// This ViewModel-Class contains Methods and Logic of SettingsWindow.
    /// Properties and Data are in SettingsWindowViewModel.Properties.cs
    /// </summary>
    public partial class SettingsWindowViewModel : ViewModelBase
    {
        public SettingsWindowViewModel()
        {
            this._settingsManager = new XmlManager();
            this._databaseLogic = new DatabaseLogic();

            this.Added = new ObservableCollection<string>();
            this.AvailableLanguages = new ObservableCollection<string>();
            this.SettingsAreas = new ObservableCollection<string>();
            this.AreDatabaseOperationsEnabled = true;

            this.SettingAreasRegistration();
            this.CommandRegistration();

            this.SelectedArea = this.SettingsAreas.FirstOrDefault();

            this.IsDatabaseProgressVisible = false;
            this.DatabaseProgress = 0;
            this.GenerateDatabaseProgressMax();
            this.GenerateDatabasePath();

            this.LoadCultures();
            this.LoadSettings();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.SaveCommand = new BaseCommand(this.Save);
            this.AddLanguageCommand = new BaseCommand(this.AddLanguage);
            this.RemoveLanguageCommand = new BaseCommand(this.RemoveLanguage);
            this.CloseCommand = new BaseCommand(this.Close);
            this.SearchSettingCommand = new BaseCommand(this.SearchSetting);
            this.ClearSearchCommand = new BaseCommand(this.ClearSearch);
            this.CreateNewDatabaseCommand = new BaseCommand(this.CreateNewDatabase);
            this.ResetDatabaseCommand = new BaseCommand(this.ResetDatabase);
            this.DeleteDatabaseCommand = new BaseCommand(this.DeleteDatabase);
        }

        // Common
        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SearchSettingCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

        // Language
        public ICommand AddLanguageCommand { get; set; }
        public ICommand RemoveLanguageCommand { get; set; }

        // Database
        public ICommand CreateNewDatabaseCommand { get; set; }
        public ICommand ResetDatabaseCommand { get; set; }
        public ICommand DeleteDatabaseCommand { get; set; }

        #region Common Commands
        private void Close(object param)
        {
            this.CloseAction.Invoke();
        }

        private void Save(object param)
        {
            Settings settings = new Settings()
            {
                AlternationColor = this.SelectedAlternationColor,
                AddedLanguages = this.Added.ToList()
            };

            if (File.Exists(settings.SettingsPath))
                this._settingsManager.UpdateSettings(settings);
            else
                this._settingsManager.SaveSettings(settings);

            CloseAction.Invoke();
        }

        private void SearchSetting(object param)
        {
            List<string> temp = new List<string>();
            temp = this.SettingsAreas.Where(w => w.ToLower().Contains(this.SearchString)).ToList();


            this.SettingsAreas.Clear();
            foreach (var t in temp)
            {
                this.SettingsAreas.Add(t);
            }
        }

        private void ClearSearch(object param)
        {
            this.SearchString = String.Empty;
            this.SettingsAreas.Clear();
            this.SettingAreasRegistration();
        }
        #endregion Common Commands

        #region Language Commands
        private void AddLanguage(object param)
        {
            if (!this.Added.Contains(this.SelectedAvailable))
            {
                this.Added.Add(this.SelectedAvailable);
            }
        }

        private void RemoveLanguage(object param)
        {
            if (this.Added.Contains(this.SelectedAdded))
            {
                this.Added.Remove(this.SelectedAdded);
            }
        }
        #endregion Language Commands

        #region Database Commands
        private async void CreateNewDatabase(object param)
        {
            try
            {
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
                        DatabaseProgress++;
                }

                for (int i = DatabaseProgressMax; i != 0; )
                {
                    DatabaseProgress = i;
                    i = i - 10;
                    await Task.Delay(5);
                }
            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
            }
        }

        private async void ResetDatabase(object param)
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
                            DatabaseProgress++;
                    }

                    for (int i = DatabaseProgressMax; i != 0; )
                    {
                        DatabaseProgress = i;
                        i = i - 10;
                        await Task.Delay(5);
                    }
                }
            }
            finally
            {
                this.AreDatabaseOperationsEnabled = true;
            }
        }

        private void DeleteDatabase(object param)
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
        #endregion Database Commands

        #endregion Commands

        #region Methods
        private async void LoadSettings()
        {
            Result<Settings> load = await this._settingsManager.LoadSettingsAsync();
            if (load.Status == Common.Status.Success)
            {
                this.SelectedAlternationColor = load.Data.AlternationColor;
                this.Added = new ObservableCollection<string>(load.Data.AddedLanguages);
            }
        }

        private void LoadCultures()
        {
            var cultures = CultureHandler.GetDistinctedCultures();
            foreach (CultureInfo culture in cultures)
            {
                this.AvailableLanguages.Add(culture.NativeName);
            }
        }

        private void SettingAreasRegistration()
        {
            this.SettingsAreas.Add("Row Appearance");
            this.SettingsAreas.Add("Languages");
            this.SettingsAreas.Add("Database Settings");
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
                    break;

                case "row appearance":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = true;
                    this.DatabaseSettingsVisibility = false;
                    break;

                case "database settings":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = false;
                    this.DatabaseSettingsVisibility = true;
                    break;

                default:
                    return;
            }
        }

        private void GenerateDatabasePath()
        {
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string savePath = String.Format("{0}\\{1}", saveDir, "smartVocDb.sqlite");

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
        #endregion Methods
    }
}