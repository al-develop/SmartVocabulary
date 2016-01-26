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

        private void LoadCulutures()
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
                    break;

                case "row appearance":
                    this.LanguagePageVisibility = false;
                    this.RowPageVisibility = true;
                    break;

                default:
                    return;
            }
        }
        #endregion Methods
    }
}