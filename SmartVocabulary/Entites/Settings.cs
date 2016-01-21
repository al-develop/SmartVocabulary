﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartVocabulary.Entites
{
    [Serializable]
    public class Settings
    {
        [XmlIgnore]
        public string SettingsDir = AppDomain.CurrentDomain.BaseDirectory + "\\Settings";

        [XmlIgnore]
        public string SettingsPath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings\\settings.xml";

        public string AlternationColor { get; set; }

        public List<string> AddedLanguages { get; set; }
        public string SelectedLanguage { get; set; }
    }
}
