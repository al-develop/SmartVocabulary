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
            this._databaseLogic = new DatabaseLogic();

            this.Added = new ObservableCollection<string>();
            this.AvailableLanguages = new ObservableCollection<string>();
            this.SettingsAreas = new ObservableCollection<string>();
            this.AreDatabaseOperationsEnabled = true;            
            this.IsDatabaseProgressVisible = false;
            this.DatabaseProgress = 0;
            this.GenerateDatabaseProgressMax();
            this.GenerateDatabasePath();
            
            if(File.Exists(this.DatabasePath))
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

        #region Common Commands
        private void Close()
        {
            this.CloseAction.Invoke();
        }

        private void Save()
        {
            Settings settings = new Settings()
            {
                AlternationColor = this.SelectedAlternationColor,
                AddedLanguages = this.Added.ToList()
            };

            if (File.Exists(settings.SettingsPath))
                SettingsLogic.Instance.UpdateSettings(settings);
            else
                SettingsLogic.Instance.SaveSettings(settings);

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

                for (int i = this.DatabaseProgressMax; i != 0; )
                {
                    this.DatabaseProgress = i;
                    i = i - 10;
                    await Task.Delay(5);
                }
                this.DatabaseProgressInPercent = "Creating done";
                this.GenerateDatabasePath();
                this.IsDatabaseExisting = true;
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

                    for (int i = this.DatabaseProgressMax; i != 0; )
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
            }
        }

        private void LoadCultures()
        {
            List<CultureInfo> cultures = CultureHandler.GetDistinctedCultures();
            foreach (CultureInfo culture in cultures)
            {
                this.AvailableLanguages.Add(culture.NativeName);
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

        private void FilterLanguages()
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

        private void FilterSettings()
        {
            this.SettingsAreas.Clear();
            if(!String.IsNullOrEmpty(this.SearchString))
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