using System;
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
        public static List<CultureInfo> GetCultures() => CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                                                    .ToList();

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

        /// <summary>
        /// Converts a System.String with an Language Name to the correspondive CultureInfo
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static CultureInfo ConvertStringToCultureInfo(string culture)
        {
            if (String.IsNullOrEmpty(culture))
                return CultureInfo.GetCultureInfo("en-EN");

            List<CultureInfo> cultures = GetCultures();
            return cultures.FirstOrDefault(f => f.NativeName.ToLower() == culture.ToLower());
        }
    }
}
