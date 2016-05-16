using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;
using SmartVocabulary.Logic.Factory;
using SmartVocabulary.Logic.Setting;

namespace SmartVocabulary.UI
{
    public class ExportWizardWindowViewModel : ViewModelBase
    {
        public Action CloseAction;

        #region Data
        /// <summary>
        /// Action to show MessageBoxes
        /// </summary>
        /// <params>
        /// string: messageBoxText
        /// string: caption
        /// string: name of MessageBoxButton. Must be convertible to MessageBoxButton Enum
        /// string: name of MessageBoxImage. Must be convertible to MessageBoxImage Enum
        /// </params>
        public Action<string, string, string, string> ShowMessageBox;

        private IManager ExportManager { get; set; }
        #endregion Data

        #region Properties
        private ExportKinds _selectedExportKind;
        private string _savePath;
        private ObservableCollection<string> _availableLanguages;
        private bool _canExportBegin;
        private bool _canSetSettings;
        private string _selectedLanguage;
        
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { SetProperty(ref _selectedLanguage, value, () => SelectedLanguage); }
        }
        public bool CanSetSettings
        {
            get { return _canSetSettings; }
            set { SetProperty(ref _canSetSettings, value, () => CanSetSettings); }
        }
        public bool CanExportBegin
        {
            get { return _canExportBegin; }
            set { SetProperty(ref _canExportBegin, value, () => CanExportBegin); }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                SetProperty(ref _availableLanguages, value, () => AvailableLanguages);
                CheckIfExportIsPossible();
            }
        }
        public string SavePath
        {
            get { return _savePath; }
            set
            {
                SetProperty(ref _savePath, value, () => SavePath);
                CheckIfExportIsPossible();
            }
        }
        public ExportKinds SelectedExportKind
        {
            get { return _selectedExportKind; }
            set
            {
                SetProperty(ref _selectedExportKind, value, () => SelectedExportKind);
                if(SelectedExportKind == ExportKinds.XML)
                    CanSetSettings = false;
                else
                    CanSetSettings = true;
            }
        }
        #endregion

        public ExportWizardWindowViewModel()
        {
            Result<Settings> settings = SettingsLogic.Instance.LoadSettings();
            this.AvailableLanguages = new ObservableCollection<string>(settings.Data?.AddedLanguages);
            this.CanExportBegin = false;

            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {
            CancelCommand = new DelegateCommand(Cancel);
            SelectPathCommand = new DelegateCommand(SelectPath);
            BeginExportCommand = new DelegateCommand(BeginExport);
        }

        public ICommand SelectPathCommand { get; set; }
        public ICommand BeginExportCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        
        private void BeginExport()
{
            var validationResult = this.ValidateBeforeExport();
            if(validationResult.Status != Status.Success)
            {
                this.ShowMessageBox.Invoke(validationResult.Message, "Error while validation", "OK", "Error");
                return;
            }

            this.ExportManager = ManagerFactory.GetManager(this.SelectedExportKind);
            List<VocableLanguageWrapper> exportList = this.GenerateExportList();
            string savePath = $"{this.SavePath}SmartVocabulary{ExportKindsExtrahator.GetExportKindExtension(this.SelectedExportKind)}";
            if(!File.Exists(savePath))
                File.Create(savePath);

            ExportManager.Export(exportList, savePath);
        }

        private void SelectPath()
        {
            var dialog = new OpenFolderDialog.FolderSelectDialog();
            dialog.Title = "Export Selection";
            if(dialog.ShowDialog() == false)
            {
                return;
            }

            this.SavePath = dialog.FileName;
        }

        private void Cancel()
        {
            this.CloseAction.Invoke();
        }
        #endregion Commands

        private void CheckIfExportIsPossible()
        {
            if(String.IsNullOrEmpty(this.SavePath))
            {
                this.CanExportBegin = false;
                return;
            }

            if(this.AvailableLanguages == null
                || this.AvailableLanguages.Count == 0)
            {
                this.CanExportBegin = false;
                return;
            }            

            this.CanExportBegin = true;
        }

        private Result ValidateBeforeExport()
        {
            if(String.IsNullOrEmpty(this.SavePath))
            {
                return new Result("save path was null or empty", Status.Error);
            }

            //if(this.SelectedLanguages == null
            //    || this.SelectedLanguages.Count == 0)
            //{
            //    return new Result("no languages for export selected", Status.Error);
            //}

            return new Result("", Status.Success);
        }

        private List<VocableLanguageWrapper> GenerateExportList()
        {
            var exportList = new List<VocableLanguageWrapper>();
            var dataLogic = new VocableLogic();

            foreach(string language in this.AvailableLanguages)
            {
                exportList.Add(new VocableLanguageWrapper()
                {
                    Language = language,
                    Vocables = dataLogic.GetAllVocables(language).Data
                });
            }

            return exportList;
        }
    }
}