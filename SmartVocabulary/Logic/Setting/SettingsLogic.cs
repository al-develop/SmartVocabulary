using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Setting
{
    public class SettingsLogic
    {
        #region Singleton
        private static readonly Lazy<SettingsLogic> _instance = new Lazy<SettingsLogic>();
        public static SettingsLogic Instance => _instance.Value;
        #endregion Singleton

        public SettingsLogic()
        {

        }

        public Result SaveSettings(Settings settings)
        {
            try
            {
                if(!Directory.Exists(settings.SettingsDir))
                    Directory.CreateDirectory(settings.SettingsDir);

                using(FileStream stream = new FileStream(settings.SettingsPath, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(stream, settings);
                }
                return new Result();
            }
            catch(Exception ex)
            {
                return new Result("Error occured on saving settings", Status.Error, ex);
            }
        }

        private Result DeleteSettings(bool deleteDirectory, string settingsPath, string settingsDir)
        {
            if(File.Exists(settingsPath))
                File.Delete(settingsPath);

            if(deleteDirectory && Directory.Exists(settingsDir))
                Directory.Delete(settingsDir);

            return new Result("", Status.Success);
        }

        public Result UpdateSettings(Settings settings)
        {
            /* Für den Update: Zuerst werden die alten Einstellungen geladen.
             * Dann werden alle Eigenschaften aus den neuen Settings die nicht NULL sind reingeschrieben
             * Dann werden die alten Settings über die Delete Methode gelöscht
             * Dann wird die SaveMethode aufgerufen um die Einstellungen zu überschreiben
             */

            Settings updateSettings = this.LoadSettings().Data;
            if(updateSettings == null)
                return new Result("Couldn't load old settings", Status.Error);

            if(settings.AddedLanguages != null)
                updateSettings.AddedLanguages = settings.AddedLanguages;

            if(settings.AlternationColor != null)
                updateSettings.AlternationColor = settings.AlternationColor;

            if(settings.SelectedLanguage != null)
                updateSettings.SelectedLanguage = settings.SelectedLanguage;

            this.DeleteSettings(false, updateSettings.SettingsPath, updateSettings.SettingsDir);
            return this.SaveSettings(updateSettings);

        }

        public Result<Settings> LoadSettings()
        {
            try
            {
                Settings settings = new Settings();
                using(FileStream stream = new FileStream(settings.SettingsPath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    settings = serializer.Deserialize(stream) as Settings;
                }
                return new Result<Settings>(settings, "Loading successful", "", Status.Success);
            }
            catch(Exception ex)
            {
                return new Result<Settings>(null, "Error occured on loading settings", Status.Error, ex);
            }
        }

        public async Task<Result<Settings>> LoadSettingsAsync() => await Task.Run(() => this.LoadSettings());
    }
}
