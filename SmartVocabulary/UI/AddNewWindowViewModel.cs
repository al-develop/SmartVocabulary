using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseMvvm;
using SmartVocabulary.Entites;

namespace SmartVocabulary.UI
{
    public class AddNewWindowViewModel : ViewModelBase
    {
        #region Properties
        private Vocable _vocable;

        public Vocable Vocable
        {
            get { return _vocable; }
            set { NotifyPropertyChanged(ref _vocable, value, () => Vocable); }
        }
        #endregion Properties

        public AddNewWindowViewModel()
        {

        }

        #region Commands
        private void CommandRegistration()
        {

        }
        #endregion Commands
				
    }
}
