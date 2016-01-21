﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;

namespace SmartVocabulary.Logic.Manager
{
    public class XmlManager : IManager
    {
        #region IManager Member

        public Result Export(IList<Vocable> VocableCollection, string savePath)
        {
            if (VocableCollection == null)
            {
                LogWriter.Instance.WriteLine("Xml Export: Vocable Collection is empty");
                return new Result("Xml Export: Vocable Collection is empty", "", Status.Error);
            }

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IList<Vocable>));
                serializer.Serialize(stream, VocableCollection);
            }


            return new Result("Xml Export: Export successfull", "", Status.Success);
        }

        public Result<IList<Vocable>> Import(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                LogWriter.Instance.WriteLine("Xml Import: File does not exist");
                return new Result<IList<Vocable>>("Xml Import: File does not exist", "", Status.Error);
            }

            var VocableCollection = new List<Vocable>();
            using (FileStream stream = new FileStream(sourcePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Vocable>));
                VocableCollection = serializer.Deserialize(stream) as List<Vocable>;
            }

            if (VocableCollection == null)
            {
                LogWriter.Instance.WriteLine("Xml Import: Collection is null");
                return new Result<IList<Vocable>>(null, "Xml Import: Collection is null", "", Status.Error);
            }

            return new Result<IList<Vocable>>(VocableCollection, "Xml Import: Import successfull", "", Status.Error);
        }

        #endregion


        public Result SaveSettings(Settings settings)
        {
            try
            {
                if (!Directory.Exists(settings.SettingsDir))
                    Directory.CreateDirectory(settings.SettingsDir);

                using (FileStream stream = new FileStream(settings.SettingsPath, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(stream, settings);
                }
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result("Error occured on saving settings", Status.Error, ex);
            }
        }

        private Result DeleteSettings(bool deleteDirectory, string settingsPath, string settingsDir)
        {
            if (File.Exists(settingsPath))
                File.Delete(settingsPath);

            if (deleteDirectory && Directory.Exists(settingsDir))
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
            if (settings.AddedLanguages != null)
                updateSettings.AddedLanguages = settings.AddedLanguages;

            if (settings.AlternationColor != null)
                updateSettings.AlternationColor = settings.AlternationColor;

            if (settings.SelectedLanguage != null)
                updateSettings.SelectedLanguage = settings.SelectedLanguage;

            this.DeleteSettings(false, updateSettings.SettingsPath, updateSettings.SettingsDir);
            return this.SaveSettings(updateSettings);

        }

        public Result<Settings> LoadSettings()
        {
            try
            {
                Settings settings = new Settings();
                using (FileStream stream = new FileStream(settings.SettingsPath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    settings = serializer.Deserialize(stream) as Settings;
                }
                return new Result<Settings>(settings, "Loading successful", "", Status.Success);
            }
            catch (Exception ex)
            {
                return new Result<Settings>(null, "Error occured on loading settings", Status.Error, ex);
            }
        }
    }
}
