using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;
using SmartVocabulary.Entites;

namespace SmartVocabulary.Logic.Factory
{
    public interface IManager
    {
        /// <summary>
        /// Method signature for export data
        /// </summary>
        /// <param name="vocableCollection">Contains the data which has to be exported
        ///     <para type="Key">  string:           Language</para>
        ///     <para type="Value">IListOfVocable:   Collection of data to the associated language</para>
        /// </param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        Result Export(List<VocableLanguageWrapper> vocableCollection, string savePath);
        Result<List<VocableLanguageWrapper>> Import(string sourcePath);
    }
}