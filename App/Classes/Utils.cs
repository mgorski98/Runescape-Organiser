using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunescapeOrganiser;

namespace Utils {
    public static class ListUtils {
        public static bool IsEmpty<T>(this List<T> list) => list.Count == 0;
        public static bool IsNullOrEmpty<T>(List<T> list) => (list == null || list.Count == 0);

        
    }

    public static class StringUtils {
        /// <summary>
        /// Checks if the string is contained of only numeric values
        /// </summary>
        /// <param name="s">
        /// String to be checked
        /// </param>
        /// <returns>
        /// True if string is numeric, False otherwise
        /// </returns>
        public static bool IsNumeric(string s) => Int64.TryParse(s, out long r);

        /// <summary>
        /// Capitalizes all words in a sentence
        /// </summary>
        /// <param name="s">
        /// String that needs its words capitalized
        /// </param>
        /// <returns>
        /// New string with all the words capitalized
        /// </returns>
        public static string CapitalizeSentenceWords(this string s) => String.Join(" ", s.Split(' ').Select(word => word.Capitalize()));

        /// <summary>
        /// Capitalizes the first word of the sentence
        /// </summary>
        /// <param name="s">
        /// Sentence to be capitalized
        /// </param>
        /// <returns>
        /// New sentence that has its words capitalized
        /// </returns>
        public static string Capitalize(this string s) => s[0].ToString().ToUpper() + s.Substring(1);

        /// <summary>
        /// Calculates the Levenshtein distance between two strings
        /// </summary>
        /// <param name="first">
        /// First string to be used in calculation
        /// </param>
        /// <param name="second">
        /// Second string to be used in calculation
        /// </param>
        /// <returns>
        /// Levenshtein distance between the two strings
        /// </returns>
        public static int LevenshteinDistance(string first, string second) {
            if (first.Length == 0) return second.Length;
            if (second.Length == 0) return first.Length;
            int m, n;
            m = first.Length + 1;
            n = second.Length + 1;
            int[,] matrix = new int[m, n];
            for (int i = 0; i < m; ++i) matrix[i, 0] = i;
            for (int j = 1; j < n; ++j) matrix[0, j] = j;
            int cost = 0;
            for (int i = 1; i < m; ++i) {
                for (int j  = 1; j < n; ++j) {
                    cost = (first[i - 1] == second[j - 1] ? 0 : 1);
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), 
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[m - 1, n - 1];
        }
    }

    public static class DictUtils {
        public static bool IsEmpty<T, U>(this Dictionary<T, U> dict) => dict.Count == 0;
        public static bool IsNullOrEmpty<T, U>(Dictionary<T, U> dict) => (dict == null || dict.Count == 0);
    }

    public static class DateUtils {
        /// <summary>
        /// Gets todays date in format day/month/year as a string
        /// </summary>
        /// <returns>
        /// New string representing date in format yyyy/mm/dd
        /// </returns>
        public static string GetTodaysDate() {
            DateTime dt = DateTime.Now;
            return String.Format(
                "{0}/{1}/{2}",
                dt.Year,
                dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString(),
                dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString()
            );
        }
    }
}
