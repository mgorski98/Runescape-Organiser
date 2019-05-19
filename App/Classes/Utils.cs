using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public static class ListUtils {
        public static bool IsEmpty<T>(this List<T> list) => list.Count == 0;
        public static bool IsNullOrEmpty<T>(List<T> list) => (list == null || list.Count == 0);
    }

    public static class StringUtils {
        public static bool IsNumeric(string s) => Int64.TryParse(s, out long r);
        public static string CapitalizeSentenceWords(this string s) => String.Join(" ", s.Split(' ').Select(word => word.Capitalize()));
        public static string Capitalize(this string s) => s[0].ToString().ToUpper() + s.Substring(1);
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
        public static string GetTodaysDate() {
            DateTime dt = DateTime.Now;
            return String.Format(
                "{0}/{1}/{2}", 
                dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString(), 
                dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString(), 
                dt.Year
            );
        }
    }
}
