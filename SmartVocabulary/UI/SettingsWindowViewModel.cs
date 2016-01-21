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
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Manager;

namespace SmartVocabulary.UI
{
    public class SettingsWindowViewModel : ViewModelBase
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
        public bool RowPageVisbility
        {
            get { return _rowPageVisibility; }
            set { NotifyPropertyChanged(ref _rowPageVisibility, value, () => RowPageVisbility); }
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

        public SettingsWindowViewModel()
        {
            _settingsManager = new XmlManager();

            this.Added = new ObservableCollection<string>();
            this.AvailableLanguages = new ObservableCollection<string>();
            this.SettingsAreas = new ObservableCollection<string>();

            this.SettingAreasRegistration();
            this.CommandRegistration();

            this.SelectedArea = this.SettingsAreas.FirstOrDefault();

            this.LoadCulutures();
            this.LoadSettings();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.SaveCommand = new BaseCommand(this.Save);
            this.AddCommand = new BaseCommand(this.AddLanguage);
            this.RemoveCommand = new BaseCommand(this.RemoveLanguage);
            this.CloseCommand = new BaseCommand(this.Close);
            this.SearchSettingCommand = new BaseCommand(this.SearchSetting);
            this.ClearSearchCommand = new BaseCommand(this.ClearSearch);
        }

        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand SearchSettingCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

        private void Close(object param)
        {
            this.CloseAction.Invoke();
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
        #endregion

        #region Methods
        private void LoadSettings()
        {
            var load = this._settingsManager.LoadSettings();
            if (load.Status == Common.Status.Success)
            {
                this.SelectedAlternationColor = load.Data.AlternationColor;
                this.Added = new ObservableCollection<string>(load.Data.AddedLanguages);
            }
        }

        private void LoadCulutures()
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            foreach (var culture in cultures)
            {
                this.AvailableLanguages.Add(culture.NativeName);
            }
        }

        private void SettingAreasRegistration()
        {
            this.SettingsAreas.Add("Row Appearance");
            this.SettingsAreas.Add("Languages");
        }

        private void AreaSelectionChanged()
        {
            if (String.IsNullOrEmpty(SelectedArea))
                return;

            switch (SelectedArea.ToLower())
            {
                case "languages":
                    this.LanguagePageVisibility = true;
                    this.RowPageVisbility = false;
                    break;

                case "row appearance":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisbility = true;
                    break;

                default:
                    return;
            }
        }
        #endregion Methods
    }
}