using System;

namespace SmartVocabulary.Entites
{
    public class Vocable 
    {
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

            return success 
                ? kind 
                : VocableKind.Unknown;
        }

        public static int SetIdDynamic(object param)
        {
            int id;
            bool success = Int32.TryParse(param.ToString(), out id);

            return success 
                ? id 
                : 0;
        }
    }
}
