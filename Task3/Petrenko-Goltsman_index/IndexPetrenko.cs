using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Petrenko_Goltsman_index
{
    class IndexPetrenko
    {
        /// <summary>
        /// Calculates Petrenko-Goltsman index based on input string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static float CalculateIndex(string inputString)
        {
            float index = 0f;
            string cleanString = GetOnlyCharacters(inputString);
            int counter = 1;

            foreach (char c in cleanString)
            {
                index += 0.5f * counter;
                counter += 2;
            }

            index *= cleanString.Length;
            return index;
        }

        /// <summary>
        /// Calculates Petrenko-Goltsman index based on input string with comment
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static float CalculateIndexWithComment(string inputString)
        {
            string beforeComment = TrimComment(inputString);
            string comment = GetComment(inputString);

            float index = CalculateIndex(beforeComment) + CalculateIndex(comment);

            return index;
        }

        /// <summary>
        /// Get string of any kind of letters, excluding everything else
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static string GetOnlyCharacters(string inputString)
        {
            //get any kind of letter from any language
            Regex regex = new Regex(@"\p{L}");
            //select all matches
            MatchCollection matches = regex.Matches(inputString);
            //cast matches to string
            IEnumerable<string> values = matches.Cast<Match>().Select(m => m.Value);
            //build a resulting string
            string output = String.Join("", values);

            return output;
        }

        /// <summary>
        /// Trims the string after '|' symbol
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string TrimComment(string inputString)
        {
            string output = inputString;
            int index = output.IndexOf("|");
            if (index >= 0)
                output = output.Substring(0, index);

            return output;
        }
        /// <summary>
        /// Trims the string before '|' symbol
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GetComment(string inputString)
        {
            string output = inputString;
            int index = output.IndexOf("|");
            if (index >= 0)
                output = output.Substring(index+1);
            else
                return string.Empty;

            return output;
        }
    }
}
