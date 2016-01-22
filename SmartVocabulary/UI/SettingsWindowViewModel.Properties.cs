using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseMvvm;
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
        public Action CloseAction { get; set; }
        #endregion

        #region Properties
        #region Visbility
        private bool _rowPageVisibility;
        private bool _languagePageVisibility;

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
        #endregion Visbility

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
        #endregion
    }
}
