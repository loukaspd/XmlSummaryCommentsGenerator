using System.Linq;
using System.Text.RegularExpressions;

namespace XmlComments4DataMembers
{
    internal static class HelperMethods
    {

        #region Regex

        /// <summary>Checks if line contains property and returns it's name</summary>
        /// <param name="lineContent"></param>
        /// <returns></returns>
        public static string ExtractProperty(this string lineContent)
        {
            if (string.IsNullOrWhiteSpace(lineContent)) return null;
            Match match = new Regex("public\\s[^\\s]*\\s([^\\s]*)\\s{\\sget;\\sset;\\s}.*").Match(lineContent);
            if (!match.Success || (match.Groups?.Count ?? 0) < 2) return null;

            return match.Groups[1].Value;
        }


        public static string ExtractDataMember(this string lineInput)
        {
            var regex = new Regex("DataMember.*Name\\s*=\\s*\"(.*)\"");
            Match match = regex.Match(lineInput);
            if (!match.Success || (match.Groups?.Count ?? 0) < 2) return null;

            return match.Groups[1].Value;
        }


        private static string GetClassName(string lineInput)
        {
            var regex = new Regex(".*public\\s*class\\s*(.*)\\s*");
            Match match = regex.Match(lineInput);
            if (!match.Success) return null;

            string dataMember = match.Groups[1].Value;

            return SplitCamelCase(dataMember);
        }


        public static bool LineIsSummaryEnd(this string lineInput)
        {
            return new Regex(".*<\\/summary>").IsMatch(lineInput);
        }

        #endregion Regex


        /// <summary>Returns the first line that is not empty prev to the specified index</summary>
        /// <param name="inputLines"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetPrevLineWithContent(string[] inputLines, int index)
        {
            while(--index > 0)
            {
                string lineContent = inputLines[index];
                if (!string.IsNullOrWhiteSpace(lineContent)) return lineContent;
            }

            return null;
        }


        #region Identation
        public static int GetLineIdentation(this string lineInput)
        {
            int result = 0;
            while (result < lineInput.Length && lineInput[result] == ' ') result++;

            return result;
        }


        public static string AddIdentetion(this string input, int identation)
        {
            return new string(Enumerable.Repeat(' ', identation).ToArray()) + input;
        }
        #endregion Identation


        #region String Methods
        public static string CamelCase(this string input)
        {
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }


        public static string SplitCamelCase(this string camelCaseInput)
        {
            if (camelCaseInput == null) return null;
            char[] result = Regex.Replace(camelCaseInput, "([A-Z])", " $1", RegexOptions.Compiled).Trim().ToCharArray();
            result[0] = char.ToUpper(result[0]);

            return new string(result);
        }
        #endregion String Methods
    }
}
