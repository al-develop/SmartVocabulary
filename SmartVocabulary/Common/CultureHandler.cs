using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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


        /// <summary>
        /// Loads a List of System.String, containing NativeNames of all CultureInfos in lower case
        /// </summary>
        /// <returns>A List of System.String, containing NativeNames of all CultureInfos in lower case</returns>
        public static List<string> GetCulturesAsLowerCaseStringCollectionByFilter()
        {
            List<CultureInfo> cultures = GetCultures();
            List<string> result = new List<string>();
            foreach (var culture in cultures)
            {
                result.Add(culture.NativeName.ToLower());
            }

            return result;
        }
    }
}
