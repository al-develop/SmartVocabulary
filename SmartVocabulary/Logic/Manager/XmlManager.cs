﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;
using SmartVocabulary.Logic.Factory;

namespace SmartVocabulary.Logic.Manager
{
    public class XmlManager : IManager
    {
        #region IManager Member

        public Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath)
        {
            if (vocableCollection == null)
            {
                LogWriter.Instance.WriteLine("Xml Export: Vocable Collection is empty");
                return new Result("Xml Export: Vocable Collection is empty", "", Status.Error);
            }
            
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("SmartVocabulary"));
            using (var saveStream = new FileStream(savePath, FileMode.Create))
            {
                foreach (var collection in vocableCollection)
                {
                    document.Element("SmartVocabulary")
                            .Add(new XElement(collection.Language));

                    int i = 0;
                    foreach (var voc in collection.Vocables)
                    {
                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("ID", collection.Vocables
                                                                  .Select(s => s.ID)
                                                                  .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Native", collection.Vocables
                                                                      .Select(s => s.Native)
                                                                      .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Kind", collection.Vocables
                                                                .Select(s => s.Kind)
                                                                .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Translation", collection.Vocables
                                                                           .Select(s => s.Translation)
                                                                           .ElementAt(i)));
                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Definition", collection.Vocables
                                                                .Select(s => s.Definition)
                                                                .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Opposite", collection.Vocables
                                                                .Select(s => s.Opposite)
                                                                .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Synonym", collection.Vocables
                                                                .Select(s => s.Synonym)
                                                                .ElementAt(i)));

                        document.Element("SmartVocabulary")
                                .Element(collection.Language)
                                .Add(new XElement("Example", collection.Vocables
                                                                .Select(s => s.Example)
                                                                .ElementAt(i)));
                        i++;
                    }
                }

                document.Save(saveStream);
            };

            return new Result("Xml Export: Export successfull", "", Status.Success);
        }

        public Result<List<VocableLanguageWrapper>> Import(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                LogWriter.Instance.WriteLine("Xml Import: File does not exist");
                return new Result<List<VocableLanguageWrapper>>("Xml Import: File does not exist", "", Status.Error);
            }

            var VocableCollection = new List<VocableLanguageWrapper>();
            //using (FileStream stream = new FileStream(sourcePath, FileMode.Create))
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(List<VocableLanguageWrapper>));
            //    VocableCollection = serializer.Deserialize(stream) as List<VocableLanguageWrapper>;
            //}

            //TODO: Create Import Logic with XDocument 
            
            using (FileStream stream = new FileStream(sourcePath, FileMode.Create))
            {
                XDocument document = XDocument.Load(stream);
            }

            if (VocableCollection == null)
            {
                LogWriter.Instance.WriteLine("Xml Import: Collection is null");
                return new Result<List<VocableLanguageWrapper>>(null, "Xml Import: Collection is null", "", Status.Error);
            }

            return new Result<List<VocableLanguageWrapper>>(VocableCollection, "Xml Import: Import successfull", "", Status.Error);
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
            if (updateSettings == null)
                return new Result("Couldn't load old settings", Status.Error);

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

        public async Task<Result<Settings>> LoadSettingsAsync()
        {
            return await Task.Run(() => this.LoadSettings());
        }
    }
}
