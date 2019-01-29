﻿using System.Linq;
using System.Text.RegularExpressions;

namespace XmlComments4DataMembers
{
    public class Implementation
    {
        public static string GetDataMember(string lineInput)
        {
            var regex = new Regex("DataMember.*Name\\s*=\\s*\"(.*)\"");
            Match match = regex.Match(lineInput);
            if (!match.Success) return null;

            string dataMember = match.Groups[1].Value;

            return SplitCamelCase(dataMember);
        }


        /// <summary>
        /// Search until you find class declaration (There might be spaces between [DataContract] and actual class declaration
        /// </summary>
        /// <param name="input">File input lines</param>
        /// <param name="startIndex">line index where [DataContract] lives</param>
        /// <returns>Class name in camel case</returns>
        public static string FindClassName(string[] input, int startIndex)
        {
            while (startIndex < input.Length)
            {
                string className = GetClassName(input[startIndex]);
                if (className != null) return className;

                startIndex++;
            }

            return null;
        }


        public static int GetIdentationSpaces(string lineInput)
        {
            int result = 0;
            while (result < lineInput.Length && lineInput[result] == ' ') result++;

            return result;
        }


        public static string[] GetXmlComment(string content, int spaces)
        {
            string identation = new string(Enumerable.Repeat(' ', spaces).ToArray());
            return new[]
            {
                $"{identation}/// <summary>",
                $"{identation}/// {content}",
                $"{identation}/// </summary>"
            };
        }

        private static string SplitCamelCase(string camelCaseInput)
        {
            char[] result = Regex.Replace(camelCaseInput, "([A-Z])", " $1", RegexOptions.Compiled).Trim().ToCharArray();
            result[0] = char.ToUpper(result[0]);

            return new string(result);
        }

        private static string GetClassName(string lineInput)
        {
            var regex = new Regex(".*public\\s*class\\s*(.*)\\s*");
            Match match = regex.Match(lineInput);
            if (!match.Success) return null;

            string dataMember = match.Groups[1].Value;

            return SplitCamelCase(dataMember);
        }
    }
}
