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
        public static bool IsNumeric(string s) {
            if (String.IsNullOrWhiteSpace(s)) return false;
            try {
                int x = Int32.Parse(s);
            } catch (FormatException) {
                return false;
            }
            return true;
        }

        public static string Capitalize(this string s) {
            string result = null;
            var words = s.Split(' ');
            for (int i = 0; i < words.Length; ++i) {
                words[i] = words[i][0].ToString().ToUpper() + words[i].Substring(1);
            }
            result = String.Join(" ", words);
            return result;
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
