using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Data;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Database
{
    /// <summary>
    /// Logic Layer. Work with DatabaseAccess class to get access to the Database
    /// </summary>
    public class VocableLogic
    {
        private readonly DatabaseAccess _access;

        public VocableLogic()
        {
            _access = new DatabaseAccess();
        }

        public Result<IList<Vocable>> GetAllVocables()
        {
            LogWriter.Instance.WriteLine("Excecute Method: \"GetAllVocables\" from class \"VocableLogic\"");

            return null;
        }

        public Result SaveVocable(Vocable entry)
        {
            LogWriter.Instance.WriteLine("Excecute Method: \"SaveVocable\" from class \"VocableLogic\"");
            Result saveResult = this._access.SaveVocable(entry);
            //if (saveResult.Status != Status.Success)
            //{
            //    string log = String.Format("Error occured in Method: \"SaveVocable\" in class \"VocableLogic\". Error message:{0}{1}", Environment.NewLine, saveResult.Message);
            //    LogWriter.Instance.WriteLine(log);
            //}

            //using(System.IO.FileStream stream = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\temp\\temp.xml", System.IO.FileMode.OpenOrCreate))
            //{
            //    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Vocable));
            //    serializer.Serialize(stream, entry);
            //}

            //return saveResult;
            return new Result();
        }
    }
}
