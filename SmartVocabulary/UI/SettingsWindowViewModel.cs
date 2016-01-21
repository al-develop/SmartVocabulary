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
        private SettingsWindow _window;
        #endregion

        #region Properties
        private string _selectedAlternationColor;
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
        public string SelectedAlternationColor
        {
            get { return _selectedAlternationColor; }
            set { NotifyPropertyChanged(ref _selectedAlternationColor, value, () => SelectedAlternationColor); }
        }
        #endregion

        public SettingsWindowViewModel(SettingsWindow window)
        {
            _settingsManager = new XmlManager();
            _window = window;

            this.Added = new ObservableCollection<string>();
            this.AvailableLanguages = new ObservableCollection<string>();

            this.CommandRegistration();
            this.LoadCulutures();
            this.LoadSettings();
        }

        #region Commands

        private void CommandRegistration()
        {
            this.SaveCommand = new BaseCommand(this.Save);
            this.AddCommand = new BaseCommand(this.AddLanguage);
            this.RemoveCommand = new BaseCommand(this.RemoveLanguage);
        }

        // Cancel doesn't require a Command, because it's hadled in the Code Behind
        public ICommand SaveCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

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

            _window.Close();
        }
        #endregion

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
    }
}
