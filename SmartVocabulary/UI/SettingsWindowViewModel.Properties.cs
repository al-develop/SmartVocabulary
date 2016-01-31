using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseMvvm;
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
            set { NotifyPropertyChanged(ref _databaseSettingsVisibility, value, () => DatabaseSettingsVisibility); }
        }
        public bool LanguagePageVisibility
        {
            get { return _languagePageVisibility; }
            set { NotifyPropertyChanged(ref _languagePageVisibility, value, () => LanguagePageVisibility); }
        }
        public bool RowPageVisibility
        {
            get { return _rowPageVisibility; }
            set { NotifyPropertyChanged(ref _rowPageVisibility, value, () => this.RowPageVisibility); }
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
                NotifyPropertyChanged(ref _searchString, value, () => SearchString);
            }
        }
        public string SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                NotifyPropertyChanged(ref _selectedArea, value, () => SelectedArea);
                AreaSelectionChanged();
            }
        }
        public ObservableCollection<string> SettingsAreas
        {
            get { return _settingsAreas; }
            set { NotifyPropertyChanged(ref _settingsAreas, value, () => SettingsAreas); }
        }
        #endregion SettingsSelection

        #region RowAlternation
        private string _selectedAlternationColor;

        public string SelectedAlternationColor
        {
            get { return _selectedAlternationColor; }
            set { NotifyPropertyChanged(ref _selectedAlternationColor, value, () => SelectedAlternationColor); }
        }
        #endregion RowAlternation

        #region LanguageSelection
        private ObservableCollection<string> _added;
        private ObservableCollection<string> _availableLanguages;
        private string _selectedAvailable;
        private string _selectedAdded;

        public string SelectedAdded
        {
            get { return _selectedAdded; }
            set { NotifyPropertyChanged(ref _selectedAdded, value, () => SelectedAdded); }
        }
        public string SelectedAvailable
        {
            get { return _selectedAvailable; }
            set { NotifyPropertyChanged(ref _selectedAvailable, value, () => SelectedAvailable); }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                NotifyPropertyChanged(ref _availableLanguages, value, () => AvailableLanguages);
            }
        }
        public ObservableCollection<string> Added
        {
            get { return _added; }
            set
            {
                NotifyPropertyChanged(ref _added, value, () => Added);
            }
        }
        #endregion LanguageSelection

        #region DatabaseSettings
        private int _databaseProgress;
        private bool _isDatabaseProgressVisible;
        private int _databaseProgressMax;
        private string _databasePath;
        private bool _areDatabaseOperationsEnabled;

        public bool AreDatabaseOperationsEnabled
        {
            get { return _areDatabaseOperationsEnabled; }
            set { NotifyPropertyChanged(ref _areDatabaseOperationsEnabled, value, () => AreDatabaseOperationsEnabled); }
        }
        public string DatabasePath
        {
            get { return _databasePath; }
            set
            {
                NotifyPropertyChanged(ref _databasePath, value, () => DatabasePath);
                if (String.IsNullOrEmpty(DatabasePath))
                    this.DatabasePath = "Database not available";
            }
        }
        public int DatabaseProgressMax
        {
            get { return _databaseProgressMax; }
            set { NotifyPropertyChanged(ref _databaseProgressMax, value, () => DatabaseProgressMax); }
        }
        public bool IsDatabaseProgressVisible
        {
            get { return _isDatabaseProgressVisible; }
            set { NotifyPropertyChanged(ref _isDatabaseProgressVisible, value, () => IsDatabaseProgressVisible); }
        }
        public int DatabaseProgress
        {
            get { return _databaseProgress; }
            set { NotifyPropertyChanged(ref _databaseProgress, value, () => DatabaseProgress); }
        }
        #endregion DatabaseSettings

        #endregion
    }
}