using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartVocabulary.Data;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Testing
{
    [TestClass]
    public class DatabaseMethods
    {
        DatabaseAccess access = new DatabaseAccess();
        string language = "English";
        
        [TestMethod]
        public void SaveVocable()
        {
            Vocable vocable = new Vocable()
            {
                Native = "UnitTest_Save",
                Translation = "UnitTest_Save",
                Definition = "UnitTest_Save",
                Example = "UnitTest_Save",
                Kind = VocableKind.Noun,
                Opposite = "UnitTest_Save",
                Synonym = "UnitTest_Save"
            };

            access.SaveVocable(vocable, language);
        }

        [TestMethod]
        public void EditVocable()
        {
            Vocable vocable = new Vocable()
            {
                ID = 1,
                Native = "UnitTest_Edit",
                Translation = "UnitTest_Edit",
                Definition = "UnitTest_Edit",
                Example = "UnitTest_Edit",
                Kind = VocableKind.Determiner,
                Opposite = "UnitTest_Edit",
                Synonym = "UnitTest_Edit"
            };


            access.UpdateVocable(vocable, language);
        }

        [TestMethod]
        public void DeleteVocable()
        {
            Vocable vocable = new Vocable()
            {
                ID = 3,
                Native = "UnitTest_Delete",
                Translation = "UnitTest_Delete",
                Definition = "UnitTest_Delete",
                Example = "UnitTest_Delete",
                Kind = VocableKind.Determiner,
                Opposite = "UnitTest_Delete",
                Synonym = "UnitTest_Delete"
            };

            access.DeleteVocable(vocable, language);
        }

        [TestMethod]
        public void LoadAll()
        {
            access.GetAllVocables(language);
        }
    }
}