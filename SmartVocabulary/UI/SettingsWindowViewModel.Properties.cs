using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Manager;

namespace SmartVocabulary.UI
{
    /// <summary>
    /// This ViewModel-Class contains Data and Properties of SettingsWindow.
    /// Methods and Logic are in SettingsWindowViewModel.cs
    /// </summary>
    public partial class SettingsWindowViewModel : ViewModelBase
    {
        #region Data
        private readonly XmlManager _settingsManager;
        private readonly DatabaseLogic _databaseLogic;
        public Action CloseAction { get; set; }
        #endregion

        #region Properties
        #region UI Visbility
        private bool _rowPageVisibility;
        private bool _languagePageVisibility;
        private bool _databaseSettingsVisibility;

        public bool DatabaseSettingsVisibility
        {
            get { return _databaseSettingsVisibility; }
            set { SetProperty(ref _databaseSettingsVisibility, value, () => DatabaseSettingsVisibility); }
        }
        public bool LanguagePageVisibility
        {
            get { return _languagePageVisibility; }
            set { SetProperty(ref _languagePageVisibility, value, () => LanguagePageVisibility); }
        }
        public bool RowPageVisibility
        {
            get { return _rowPageVisibility; }
            set { SetProperty(ref _rowPageVisibility, value, () => this.RowPageVisibility); }
        }
        #endregion UI Visbility

        #region SettingsSelection
        private ObservableCollection<string> _settingsAreas;
        private string _selectedArea;
        private string _searchString;

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                SetProperty(ref _searchString, value, () => SearchString);
                this.FilterSettings();
            }
        }
        public string SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                SetProperty(ref _selectedArea, value, () => SelectedArea);
                AreaSelectionChanged();
            }
        }
        public ObservableCollection<string> SettingsAreas
        {
            get { return _settingsAreas; }
            set { SetProperty(ref _settingsAreas, value, () => SettingsAreas); }
        }
        #endregion SettingsSelection

        #region RowAlternation
        private string _selectedAlternationColor;

        public string SelectedAlternationColor
        {
            get { return _selectedAlternationColor; }
            set { SetProperty(ref _selectedAlternationColor, value, () => SelectedAlternationColor); }
        }
        #endregion RowAlternation

        #region LanguageSelection
        private ObservableCollection<string> _added;
        private ObservableCollection<string> _availableLanguages;
        private string _selectedAvailable;
        private string _selectedAdded;
        private string _languageSearchText;

        public string LanguageSearchText
        {
            get { return _languageSearchText; }
            set 
            { 
                SetProperty(ref _languageSearchText, value, () => LanguageSearchText);
                // The Filter-Method checks if the string is null or empty, so it doesn't have to be chekced here
                // if the string is null/empty, then all cultures have to be loaded - happens in the method as well
                this.FilterLanguages();
                
            }
        }
        public string SelectedAdded
        {
            get { return _selectedAdded; }
            set { SetProperty(ref _selectedAdded, value, () => SelectedAdded); }
        }
        public string SelectedAvailable
        {
            get { return _selectedAvailable; }
            set { SetProperty(ref _selectedAvailable, value, () => SelectedAvailable); }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                SetProperty(ref _availableLanguages, value, () => AvailableLanguages);
            }
        }
        public ObservableCollection<string> Added
        {
            get { return _added; }
            set
            {
                SetProperty(ref _added, value, () => Added);
            }
        }
        #endregion LanguageSelection

        #region DatabaseSettings
        private int _databaseProgress;
        private bool _isDatabaseProgressVisible;
        private int _databaseProgressMax;
        private string _databasePath;
        private bool _areDatabaseOperationsEnabled;
        private string _databaseProgressInPercent;

        public string DatabaseProgressInPercent
        {
            get { return _databaseProgressInPercent; }
            set { SetProperty(ref _databaseProgressInPercent, value, () => DatabaseProgressInPercent); }
        }
        public bool AreDatabaseOperationsEnabled
        {
            get { return _areDatabaseOperationsEnabled; }
            set { SetProperty(ref _areDatabaseOperationsEnabled, value, () => AreDatabaseOperationsEnabled); }
        }
        public string DatabasePath
        {
            get { return _databasePath; }
            set
            {
                SetProperty(ref _databasePath, value, () => DatabasePath);
                if (String.IsNullOrEmpty(DatabasePath))
                    this.DatabasePath = "Database not available";
            }
        }
        public int DatabaseProgressMax
        {
            get { return _databaseProgressMax; }
            set { SetProperty(ref _databaseProgressMax, value, () => DatabaseProgressMax); }
        }
        public bool IsDatabaseProgressVisible
        {
            get { return _isDatabaseProgressVisible; }
            set { SetProperty(ref _isDatabaseProgressVisible, value, () => IsDatabaseProgressVisible); }
        }
        public int DatabaseProgress
        {
            get { return _databaseProgress; }
            set { SetProperty(ref _databaseProgress, value, () => DatabaseProgress); }
        }
        #endregion DatabaseSettings

        #endregion
    }
}