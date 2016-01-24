using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Entites
{
    public class Vocable 
    {
        //public Vocable()
        //{
        //    ID++;
        //}

        public int ID { get; set; }
        public string Native { get; set; }
        public string Translation { get; set; }
        public string Definition { get; set; }
        public VocableKind Kind { get; set; }
        public string Synonym { get; set; }
        public string Opposite { get; set; }
        public string Example { get; set; }
    }
}
