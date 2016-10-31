using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        /// <returns>
        /// string: the pressed MessabeBoxButton
        /// </returns>
        public Func<string, string, string, string, string> ShowMessageBox;
        public Action CloseAction;
        public Func<bool, string> ShowFolderBrowseDialogAction;

        private IManager ExportManager { get; set; }
        #endregion Data

        #region Properties
        private ExportKinds _selectedExportKind;
        private string _savePath;
        private ObservableCollection<string> _availableLanguages;
        private bool _canExportBegin;
        private string _selectedLanguage;
        private ObservableCollection<string> _selectedLanguages;
        private bool _isLanguageListSelected;
        private bool _isExportBusy;

        public bool IsExportBusy
        {
            get { return _isExportBusy; }
            set { SetProperty(ref _isExportBusy, value, () => IsExportBusy); }
        }
        public bool IsLanguageListSelected
        {
            get { return _isLanguageListSelected; }
            set
            {
                SetProperty(ref _isLanguageListSelected, value, () => IsLanguageListSelected);
                if (IsLanguageListSelected)
                    this.SelectAllLanguagesCommand.Execute(null);
                else
                    this.SelectNoneLanguagesCommand.Execute(null);
            }
        }
        public ObservableCollection<string> SelectedLanguages
        {
            get { return _selectedLanguages; }
            set { SetProperty(ref _selectedLanguages, value, () => SelectedLanguages); }
        }
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { SetProperty(ref _selectedLanguage, value, () => SelectedLanguage); }
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
                //this.UpdateSavePathExtension();
            }
        }
        #endregion

        public ExportWizardWindowViewModel()
        {
            Result<Settings> settings = SettingsLogic.Instance.LoadSettings();
            this.AvailableLanguages = new ObservableCollection<string>(settings.Data?.AddedLanguages);
            this.SelectedLanguages = new ObservableCollection<string>();

            this.CanExportBegin = false;

            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {
            CancelCommand = new DelegateCommand(Cancel);
            SelectPathCommand = new DelegateCommand(SelectPath);
            BeginExportCommand = new DelegateCommand(BeginExport);
            SelectNoneLanguagesCommand = new DelegateCommand(SelectNoneLanguages);
            SelectAllLanguagesCommand = new DelegateCommand(SelectAllLanguages);
        }

        public DelegateCommand SelectPathCommand { get; set; }
        public DelegateCommand BeginExportCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SelectNoneLanguagesCommand { get; set; }
        public DelegateCommand SelectAllLanguagesCommand { get; set; }
        
        private void SelectAllLanguages()
        {
            SelectedLanguages.Clear();
            foreach(var lang in AvailableLanguages)
            {
                SelectedLanguages.Add(lang);
            }
        }

        private void SelectNoneLanguages()
        {
            this.SelectedLanguages.Clear();
        }

        private void BeginExport()
        {
            try
            {
                IsExportBusy = true;
                var validationResult = this.ValidateBeforeExport();
                if (validationResult.Status != Status.Success)
                {
                    this.ShowMessageBox.Invoke(validationResult.Message, "Error while validation", "OK", "Error");
                    return;
                }

                this.ExportManager = ManagerFactory.GetManager(this.SelectedExportKind);
                List<VocableLanguageWrapper> exportList = this.GenerateExportList();
                //string savePath = $"{this.SavePath}SmartVocabulary{ExportKindsExtrahator.GetExportKindExtension(this.SelectedExportKind)}";
                //if (!File.Exists(this.SavePath))
                //    File.Create(this.SavePath);

                var result = (ExportManager.Export(exportList, this.SavePath));
                if (result.Status != Status.Success)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine($"Message\t\t:{result.Message}");
                    builder.AppendLine($"InnerMessage:\t{result.InnerMessage ?? string.Empty}");
                    builder.AppendLine($"Exception:\t\t{result.Exception ?? null}");

                    LogWriter.Instance.WriteLine(builder.ToString());
                    this.ShowMessageBox.Invoke(result.Message, "Error while exporting", "OK", "Error");
                    return;
                }
            }
            finally
            {
                IsExportBusy = false;
            }

            string messageBoxResult = this.ShowMessageBox.Invoke("Export successful. Do you want to open the exported file?", "Success", "YesNo", "Information");
            if(messageBoxResult.ToLower() == "yes")
            {
                Process.Start(this.SavePath);
            }
        }

        private void SelectPath()
        {
            string result = this.ShowFolderBrowseDialogAction.Invoke(true);
            if(String.IsNullOrEmpty(result))
                return;

            //string extension = ExportKindsExtrahator.GetExportKindExtension(this.SelectedExportKind);
            //this.SavePath = $"{result}SmartVocabulary_{DateTime.Now.ToShortDateString()}{extension}";
            this.SavePath = $"{result}";

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

            if (this.SelectedLanguages == null
                || this.SelectedLanguages.Count == 0)
            {
                return new Result("no languages for export selected", Status.Error);
            }

            return new Result("", Status.Success);
        }

        private void UpdateSavePathExtension()
        {            
            if(String.IsNullOrEmpty(this.SavePath))
                return;

            string extension = ExportKindsExtrahator.GetExportKindExtension(this.SelectedExportKind);
            this.SavePath = SavePath.Remove(SavePath.LastIndexOf('.'));
            this.SavePath += extension;            
        }

        private List<VocableLanguageWrapper> GenerateExportList()
        {
            var exportList = new List<VocableLanguageWrapper>();
            var dataLogic = new VocableLogic();

            foreach(string language in this.SelectedLanguages)
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