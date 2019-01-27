using System.Linq;
using System.Text.RegularExpressions;

namespace XmlComments4DataMembers
{
    public class Implementation
    {
        public static string GetDataMember(string lineInput)
        {
            if (lineInput.Length < 12) return null;

            var regex = new Regex("DataMember.*Name\\s*=\\s*\"(.*)\"");
            var match = regex.Match(lineInput);
            if (!match.Success) return null;

            string dataMember = match.Groups[1].Value;

            return SplitCamelCase(dataMember);
        }


        public static int GetIdentationSpaces(string lineInput)
        {
            int result = 0;
            while (result < lineInput.Length && lineInput[result] == ' ') result++;

            return result;
        }


        public static string[] GetXmlComment(string dataMemberName, int spaces)
        {
            string identation = new string(Enumerable.Repeat(' ', spaces).ToArray());
            return new[]
            {
                $"{identation}/// <summary>",
                $"{identation}/// {dataMemberName}",
                $"{identation}/// </summary>"
            };
        }

        private static string SplitCamelCase(string camelCaseInput)
        {
            char[] result = Regex.Replace(camelCaseInput, "([A-Z])", " $1", RegexOptions.Compiled).Trim().ToCharArray();
            result[0] = char.ToUpper(result[0]);

            return new string(result);
        }
    }
}
