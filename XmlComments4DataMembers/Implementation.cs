using Microsoft.VisualStudio.TextManager.Interop;
using System.Collections.Generic;

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
                // Check property
                string propertyName = LineContainsPropertyWithoutDataMember(inputLines, i);
                if (propertyName != null)
                {
                    int identation = inputLines[i].GetLineIdentation();
                    string dataMemberLine = GenerateDataMember(propertyName, identation);
                    List<string> xmlComments = GetXmlComment(propertyName, identation);

                    xmlComments.Add(dataMemberLine);

                    textLines.AddLines(i + addedLines, xmlComments.ToArray());
                    addedLines += xmlComments.Count;
                    continue;
                }


                string dataMember = LineContainsDataMemberWithoutSummary(inputLines, i);
                if (dataMember != null)
                {
                    int identation = inputLines[i].GetLineIdentation();
                    List<string> xmlComments = GetXmlComment(dataMember, identation);

                    textLines.AddLines(i + addedLines, xmlComments.ToArray());
                    addedLines += xmlComments.Count;
                    continue;
                }
            }
        }


        #region Private Implementation

        /// <summary>
        /// Checks if line at <paramref name="index"/> contains Property and [DataMember] does not exist in prev lines
        /// </summary>
        /// <param name="inputLines">input with all lines</param>
        /// <param name="index">The line of the index</param>
        /// <returns>The name of the property of null, if no [DataMember] is needed</returns>
        private static string LineContainsPropertyWithoutDataMember(string[] inputLines, int index)
        {
            string lineContent = inputLines[index];
            string propertyName = HelperMethods.ExtractProperty(lineContent);
            if (propertyName == null) return null;

            string prevLine = HelperMethods.GetPrevLineWithContent(inputLines, index);
            if (prevLine == null) return propertyName;

            if (HelperMethods.ExtractDataMember(prevLine) != null) return null; //already contains DataMember
            return propertyName;
        }


        /// <summary>
        /// Checks if line at <paramref name="index"/> contains [DataMember] and no <summary> exists in prev lines
        /// </summary>
        /// <param name="inputLines">input with all lines</param>
        /// <param name="index">The line of the index</param>
        /// <returns>The name of the property of null, if no [DataMember] is needed</returns>
        private static string LineContainsDataMemberWithoutSummary(string[] inputLines, int index)
        {
            string lineContent = inputLines[index];
            string dataMember = HelperMethods.ExtractDataMember(lineContent);
            if (dataMember == null) return null;

            string prevLine = HelperMethods.GetPrevLineWithContent(inputLines, index);
            if (prevLine == null) return dataMember;

            if (HelperMethods.LineIsSummaryEnd(prevLine)) return null;  //already contains Summary

            return dataMember;
        }


        /// <summary>
        /// Generate DataMember for the specified property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string GenerateDataMember(string propertyName, int identation)
        {
            return $"[DataMember(Name = \"{propertyName.CamelCase()}\")]".AddIdentetion(identation);
        }


        private static List<string> GetXmlComment(string dataMemberName, int spaces)
        {
            string description = HelperMethods.SplitCamelCase(dataMemberName);

            return new List<string>
            {
                "/// <summary>".AddIdentetion(spaces),
                $"/// {description}".AddIdentetion(spaces),
                "/// </summary>".AddIdentetion(spaces)
            };
        }

        #endregion Private Implementation
    }
}
