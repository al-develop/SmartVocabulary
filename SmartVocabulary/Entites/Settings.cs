using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
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

        // Text to Speech Settings
        public VoiceGender VoiceGender { get; set; }
        public VoiceAge VoiceAge { get; set; }
    }
}
