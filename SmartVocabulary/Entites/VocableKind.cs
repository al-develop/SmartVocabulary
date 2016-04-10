using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVocabulary.Common;

namespace SmartVocabulary.Entites
{
    //public class VocableKind
    //{
    //    public string Noun { get { return "Noun"; }  }
    //    public string Verb { get { return "Verb"; }  }
    //    public string Adjective { get { return "Adjective"; }  }
    //    public string Conjunction { get { return "Conjunction"; }  }
    //    public string Pronoun { get { return "Pronoun"; }  }
    //    public string Sentence { get { return "Sentence"; }  }
    //    public string Adverb { get { return "Adverb"; }  }
    //    public string Preposition { get { return "Preposition"; }  }
    //    public string Determiner { get { return "Determiner"; }  }
    //    public string Exclamation { get { return "Exclamation"; }  }
    //    public string Unknown { get { return "Unknown"; }  }
    //}

    public enum VocableKind
    {
        Noun,
        Verb,
        Adjective,
        Conjunction,
        Pronoun,
        Sentence,
        Phrase,
        Adverb,
        Preposition,
        Determiner,
        Exclamation,
        Unknown
    }
}