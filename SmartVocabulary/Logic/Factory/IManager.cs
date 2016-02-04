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
        Result Export(IList<Vocable> vocableCollection, string savePath);
        Result<IList<Vocable>> Import(string sourcePath);
    }
}