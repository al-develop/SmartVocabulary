using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Entites
{
    public static class ConstantSettingAreas
    {
        public const string RowAppearance = "Row Appearance";
        public const string Languages = "Languages";
        public const string DatabaseSettings = "Database Settings";

        public static List<string> GetAllSettingAreas()
        {
            return new List<string>()
            {
                RowAppearance, Languages, DatabaseSettings
            };
        }
    }
}