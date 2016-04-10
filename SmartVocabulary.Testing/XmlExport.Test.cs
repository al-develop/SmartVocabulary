using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartVocabulary.Testing
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für XmlExport
    /// </summary>
    [TestClass]
    public class XmlExport
    {
        private SmartVocabulary.Data.DatabaseAccess _dbAccess;
        public XmlExport()
        {
            _dbAccess = new Data.DatabaseAccess();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Zusätzliche Testattribute
        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ExportXmlWithSerializer()
        {
            var english = _dbAccess.GetAllVocables("English");
            var german = _dbAccess.GetAllVocables("Deutsch");
            var xmlLogic = SmartVocabulary.Logic.Factory.ManagerFactory.GetManager(Entites.ExportKinds.XML);

            var wrappedList = new List<Entites.VocableLanguageWrapper>() 
            {            
                new Entites.VocableLanguageWrapper()
                {
                    Language = "English",
                    Vocables = english.Data            
                },
                new Entites.VocableLanguageWrapper()
                {
                    Language = "Deutsch",
                    Vocables = german.Data
                }
            };

            xmlLogic.Export(wrappedList, "SerializerTest.xml");
        }

        [TestMethod]
        public void ExportXmlWithXDocument()
        {
            var english = _dbAccess.GetAllVocables("English");
            var german = _dbAccess.GetAllVocables("Deutsch");
            var xmlLogic = SmartVocabulary.Logic.Factory.ManagerFactory.GetManager(Entites.ExportKinds.XML);

            var wrappedList = new List<Entites.VocableLanguageWrapper>() 
            {            
                new Entites.VocableLanguageWrapper()
                {
                    Language = "English",
                    Vocables = english.Data            
                },
                new Entites.VocableLanguageWrapper()
                {
                    Language = "Deutsch",
                    Vocables = german.Data
                }
            };

            xmlLogic.Export(wrappedList, "XDocTest.xml");
        }
    }
}
