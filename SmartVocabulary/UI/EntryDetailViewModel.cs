﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Database;

namespace SmartVocabulary.UI
{
    public class EntryDetailViewModel : ViewModelBase
    {
        private readonly VocableLogic _logic;
        public Action CloseAction { get; set; }
        private MainWindowViewModel _parent { get; set; }

        #region Properties
        private Vocable _entry;
        private string _language;

        public string Language
        {
            get { return _language; }
            set { SetProperty(ref _language, value, () => Language); }
        }
        public Vocable Entry
        {
            get { return _entry; }
            set { SetProperty(ref _entry, value, () => Entry); }
        }
        #endregion Properties

        public EntryDetailViewModel(VocableLogic logic, string selectedLanguage, MainWindowViewModel parent, Vocable entry = null)
        {
            this._logic = logic;
            this._parent = parent;

            if (entry == null)
                Entry = new Vocable();
            else
                Entry = entry;

            this.Language = selectedLanguage;
            this.CommandRegistration();
        }

        #region Commands
        private void CommandRegistration()
        {
            this.SaveCommand = new DelegateCommand(this.Save);
            this.CancelCommand = new DelegateCommand(this.Cancel);
        }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void Cancel()
        {
            this.CloseAction.Invoke();
        }

        private void Save()
        {
            // call DB like in MainViewModel
            if (Entry.ID == 0)
            {
                // new vocable - call method to save
                this._logic.SaveVocable(this.Entry, this.Language);

                this._parent.SelectedVocable = null;
                this._parent.RibbonRefreshCommand.Execute(null);
                this._parent.Vocables.Add(new Vocable());
                this._parent.Vocables.OrderBy(o => o.ID);
            }
            else
            {
                // vocable was eidted - call update method
                this._logic.UpdateVocable(this.Entry, this.Language);
            }            

            this.CloseAction.Invoke();
        }
        #endregion Commands
				
    }
}