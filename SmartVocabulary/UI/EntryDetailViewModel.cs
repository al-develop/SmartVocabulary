using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BaseMvvm;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;

namespace SmartVocabulary.UI
{
    public class EntryDetailViewModel : ViewModelBase
    {
        private readonly VocableLogic _logic;
        public Action CloseAction { get; set; }

        #region Properties
        private Vocable _entry;

        public Vocable Entry
        {
            get { return _entry; }
            set { NotifyPropertyChanged(ref _entry, value, () => Entry); }
        }
        #endregion Properties

        public EntryDetailViewModel(VocableLogic logic, Vocable entry = null)
        {
            this._logic = logic;

            if (entry != null)
                Entry = new Vocable();
            else
                Entry = entry;

            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {

        }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void Cancel(object param)
        {
            this.CloseAction.Invoke();
        }

        private void Save(object param)
        {
            // call DB like in MainViewModel
            this.CloseAction.Invoke();
        }
        #endregion Commands
				
    }
}