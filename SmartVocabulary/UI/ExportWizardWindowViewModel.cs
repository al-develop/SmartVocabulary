using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BaseMvvm;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;

namespace SmartVocabulary.UI
{
    public class ExportWizardWindowViewModel : ViewModelBase
    {
        #region Data
        public Action CloseAction;
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
        private ObservableCollection<string> _selectedLanguages;
        private bool _canExportBegin;

        public bool CanExportBegin
        {
            get { return _canExportBegin; }
            set { NotifyPropertyChanged(ref _canExportBegin, value, () => CanExportBegin); }
        }
        public ObservableCollection<string> SelectedLanguages
        {
            get { return _selectedLanguages; }
            set
            {
                NotifyPropertyChanged(ref _selectedLanguages, value, () => SelectedLanguages);
                CheckIfExportIsPossible();
            }
        }
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set { NotifyPropertyChanged(ref _availableLanguages, value, () => AvailableLanguages); }
        }
        public string SavePath
        {
            get { return _savePath; }
            set
            {
                NotifyPropertyChanged(ref _savePath, value, () => SavePath);
                CheckIfExportIsPossible();
            }
        }
        public ExportKinds SelectedExportKind
        {
            get { return _selectedExportKind; }
            set
            {
                NotifyPropertyChanged(ref _selectedExportKind, value, () => SelectedExportKind);
            }
        }
        #endregion

        public ExportWizardWindowViewModel(List<string> availableLanguages)
        {

            this.AvailableLanguages = new ObservableCollection<string>(availableLanguages);
            this.CanExportBegin = false;
        }

        #region Commands
        private void CommandRegistration()
        {
            SelectPathCommand = new BaseCommand(this.SelectPath);
            CancelCommand = new BaseCommand(this.Cancel);
            BeginExportCommand = new BaseCommand(this.BeginExport);
        }

        public ICommand SelectPathCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand BeginExportCommand { get; set; }

        private void BeginExport(object param)
        {
            var validationResult = this.ValidateBeforeExport();
            /* TODO: - use validation Result  to check if everything's alright
             *       - Bind IsEnabled of BeginExport-Button and only enable it, 
             *              if all other controls (savepath, languages etc) have a value
             *       - convert the selected export kind to the corresponding enumeration and
             *              pass it to the Manager Factory
             *       - call export method from ManagerClass and validate the resulting Result.State
             *               
             * 
             */

            if (validationResult.Status != Status.Success)
            {
                this.ShowMessageBox.Invoke(validationResult.Message, "Error while validation", "OK", "Error");
                return;
            }

            this.ExportManager = ManagerFactory.GetManager(this.SelectedExportKind);
            // TODO: Begin Export now            

        }

        private void Cancel(object param)
        {
            this.CloseAction.Invoke();
        }

        private void SelectPath(object param)
        {
            var dialog = new OpenFolderDialog.FolderSelectDialog();
            dialog.Title = "Export Selection";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            this.SavePath = dialog.FileName;
        }
        #endregion Commands

        private void CheckIfExportIsPossible()
        {
            if (String.IsNullOrEmpty(this.SavePath))
            {
                this.CanExportBegin = false;
                return;
            }

            if (this.SelectedLanguages == null
                || this.SelectedLanguages.Count == 0)
            {
                this.CanExportBegin = false;
                return;
            }

            this.CanExportBegin = true;
        }

        private Result ValidateBeforeExport()
        {
            if (String.IsNullOrEmpty(this.SavePath))
            {
                return new Result("save path was null or empty", Status.Error);
            }

            if (this.SelectedLanguages == null
                || this.SelectedLanguages.Count == 0)
            {
                return new Result("no laguages for export selected", Status.Error);
            }

            return new Result("", Status.Success);
        }
    }
}