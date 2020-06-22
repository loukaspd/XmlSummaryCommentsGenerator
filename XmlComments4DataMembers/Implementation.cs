using Microsoft.VisualStudio.TextManager.Interop;
using System.Linq;
using System.Text.RegularExpressions;

namespace XmlComments4DataMembers
{
    public class Implementation
    {
        /// <summary>
        /// Main Method, processes input current file content
        /// </summary>
        /// <param name="textLines"></param>
        /// <param name="inputLines">The content of the </param>
        public static void ProcessCurrentFile(IVsTextLines textLines, string[] inputLines)
        {
            int addedLines = 0;
            for (int i = 0; i < inputLines.Length; i++)
            {
                // Check if summary exists already
                if (i > 0 && inputLines[i - 1].Contains("</summary>")) continue;
                // Check if line contains datamember
                string xmlContent = Implementation.GetDataMember(inputLines[i]);
                // check if line contains DataContract
                if (xmlContent == null && inputLines[i].Contains("DataContract"))
                {
                    xmlContent = Implementation.FindClassName(inputLines, i + 1);
                }

                if (xmlContent == null) continue;

                int identation = Implementation.GetIdentationSpaces(inputLines[i]);
                string[] xmlComment = Implementation.GetXmlComment(xmlContent, identation);
                textLines.AddLines(i + addedLines, xmlComment);
                addedLines += xmlComment.Length;
            }
        }


        #region Private Implementation

        private static string GetDataMember(string lineInput)
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
        private static string FindClassName(string[] input, int startIndex)
        {
            while (startIndex < input.Length)
            {
                string className = GetClassName(input[startIndex]);
                if (className != null) return className;

                startIndex++;
            }

            return null;
        }


        private static int GetIdentationSpaces(string lineInput)
        {
            int result = 0;
            while (result < lineInput.Length && lineInput[result] == ' ') result++;

            return result;
        }


        private static string[] GetXmlComment(string content, int spaces)
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

        #endregion Private Implementation
    }
}
