using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;

namespace XmlComments4DataMembers
{
    public static class VsExtensionHelpers
    {

        #region Document 

        public static Document GetCurrentDocument(this IAsyncServiceProvider serviceProvider)
        {
            var applicationObject = serviceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;
            return applicationObject?.ActiveDocument;
        }


        public static TextDocument GetTextDocument(this Document document)
        {
            if (document == null) return null;
            return document.Object("TextDocument") as TextDocument;
        }


        public static TextDocument GetCurrentTextDocument(this IAsyncServiceProvider serviceProvider)
        {
            Document currentDocument = serviceProvider.GetCurrentDocument();
            return currentDocument.GetTextDocument();
        }

        #endregion Document


        #region TextView

        public static IVsTextView GetTextView(this IAsyncServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;
            var textManager = service as IVsTextManager2;

            textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out IVsTextView view);
            return view;
        }

        #endregion TextView

        #region IVsTextLines


        public static IVsTextLines GetTextLines(this IAsyncServiceProvider serviceProvider)
        {
            IVsTextView textView = serviceProvider.GetTextView();
            if (textView == null) return null;

            textView.GetBuffer(out IVsTextLines buffer);
            return buffer;
        }


        public static void ReplaceAllText(this IVsTextLines lines, string newInput)
        {
            lines.GetLastLineIndex(out var lastLine, out var lastIndex);
            TextSpan[] _ = new TextSpan[0];
            lines.ReplaceLines(0, 0, lastLine, lastIndex, Marshal.StringToHGlobalUni(newInput), newInput.Length, _);
        }


        public static string GetAllText(this IVsTextLines lines)
        {
            lines.GetLastLineIndex(out var lastLine, out var lastIndex);
            lines.GetLineText(0, 0, lastLine, lastIndex, out var text);

            return text;
        }

        public static string[] GetAllLines(this IVsTextLines lines)
        {
            return lines.GetAllText().Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
        }


        public static void AppendTextToLine(this IVsTextLines lines, int lineIndex, string content)
        {
            lines.CreateEditPoint(lineIndex, 0, out var outVal);
            var editPoint = outVal as EditPoint;
            if (editPoint == null) return;

            editPoint.EndOfLine();
            editPoint.Insert(content);
        }


        public static void AddLine(this IVsTextLines lines, int lineIndex, string content)
        {
            lines.CreateEditPoint(lineIndex, 0, out var outVal);
            var editPoint = outVal as EditPoint;
            if (editPoint == null) return;

            editPoint.Insert(content);
            editPoint.Insert(Environment.NewLine);
        }

        public static void AddLines(this IVsTextLines lines, int lineIndex, string[] newLines)
        {
            lines.CreateEditPoint(lineIndex, 0, out var outVal);
            var editPoint = outVal as EditPoint;
            if (editPoint == null) return;

            foreach (string lineContent in newLines)
            {
                editPoint.Insert(lineContent);
                editPoint.Insert(Environment.NewLine);
            }
        }

        #endregion IVsTextLines


        public static void ShowMessageBox(this System.IServiceProvider package, string msg, string title = null)
        {
            VsShellUtilities.ShowMessageBox(
                package,
                "done",
                "Title",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
