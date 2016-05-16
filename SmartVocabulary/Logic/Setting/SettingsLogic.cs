using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SmartVocabulary.Common;
using SmartVocabulary.Data;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Setting
{
    public class SettingsLogic
    {
        #region Data
        private static SettingsAccess _settings;
        #endregion Data

        #region Singleton
        private static readonly Lazy<SettingsLogic> _instance = new Lazy<SettingsLogic>();
        public static SettingsLogic Instance => _instance.Value;
        #endregion Singleton

        public SettingsLogic()
        {
            _settings = new SettingsAccess();
        }

        public Result SaveSettings(Settings settings) => _settings.SaveSettings(settings);

        public Result UpdateSettings(Settings settings) => _settings.UpdateSettings(settings);

        public Result<Settings> LoadSettings() => _settings.LoadSettings();

        public async Task<Result<Settings>> LoadSettingsAsync() => await _settings.LoadSettingsAsync();
    }
}
