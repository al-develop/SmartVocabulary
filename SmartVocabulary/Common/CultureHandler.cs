using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Common
{
    /// <summary>
    /// Class to load Neutral Cultures and to distinct them
    /// </summary>
    public class CultureHandler
    {
        /// <summary>
        /// Loads all NeutralCultures
        /// </summary>
        /// <returns>A List of CultureInfo</returns>
        public static List<CultureInfo> GetCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                              .ToList();
        }

        /// <summary>
        /// Distincts a List of CultureInfo
        /// </summary>
        /// <returns>A List of CultureInfo</returns>
        public static List<CultureInfo> GetDistinctedCultures()
        {
            List<CultureInfo> cultures = GetCultures();
            return cultures.GroupBy(d => d.NativeName)
                           .Select(g => g.First())
                           .ToList();
        }

    }
}
