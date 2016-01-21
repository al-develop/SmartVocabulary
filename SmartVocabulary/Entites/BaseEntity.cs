using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Entites
{
    public abstract class BaseEntity
    {
        private int _id;
        public int ID { get { return _id; } }

        public BaseEntity()
        {
            _id++;
        }
    }
}
