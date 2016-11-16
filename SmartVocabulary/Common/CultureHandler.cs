using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
            LogWriter.Instance.WriteLine("Loading cultures - CultureHandler;GetCultures()");
            try
            {
                return CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                  .ToList();
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Error occured in loading cultures - CultureHandler;GetCultures()");
                errorMessage.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.AppendLine(ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorMessage.ToString());

                return CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList();
            }
        }

        /// <summary>
        /// Distincts a List of CultureInfo
        /// </summary>
        /// <returns>A List of CultureInfo</returns>
        public static List<CultureInfo> GetDistinctedCultures()
        {
            LogWriter.Instance.WriteLine("CultureHandler;GetDistinctedCultures()");
            try
            {

                List<CultureInfo> cultures = GetCultures();
                return cultures.GroupBy(d => d.EnglishName)
                               .Select(g => g.First())
                               .ToList();
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Error occured in distincting cultures - CultureHandler;GetDistinctedCultures()");
                errorMessage.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.AppendLine(ex.InnerException.Message);

                LogWriter.Instance.WriteLine(errorMessage.ToString());

                return CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures)
                                  .GroupBy(d => d.EnglishName)
                                  .Select(g => g.First())
                                  .ToList();
            }
        }


        /// <summary>
        /// Loads a List of System.String, containing EnglishNames of all CultureInfos in lower case
        /// </summary>
        /// <returns>A List of System.String, containing EnglishNames of all CultureInfos in lower case</returns>
        public static List<string> GetCulturesAsLowerCaseStringCollectionByFilter()
        {
            List<CultureInfo> cultures = GetCultures();
            List<string> result = new List<string>();
            foreach (var culture in cultures)
            {
                result.Add(culture.EnglishName.ToLower());
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
            return cultures.FirstOrDefault(f => f.EnglishName.ToLower() == culture.ToLower());
        }
    }
}
