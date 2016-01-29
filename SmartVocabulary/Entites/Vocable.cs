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


        public static VocableKind ConvertStringToKind(string param)
        {
            VocableKind kind;
            bool success = Enum.TryParse<VocableKind>(param, out kind);

            if (success)
                return kind;
            else
                return VocableKind.Unknown;
        }

        public static int SetIdDynamic(object param)
        {
            int id = 0;
            bool success = Int32.TryParse(param.ToString(), out id);

            if (success)
                return id;
            else
                return 0;
        }
    }
}
