using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ark.Models;
using System.IO;

namespace Ark.IO
{
    class ioNoteTextSaver : ioObject
    {
        public enum TextAddMethod
        {
            Append,
            Prepend
        }

        public void PasteTextToWord(string text, Word word, TextAddMethod textAddMethod = TextAddMethod.Prepend)
        {
            if (word.Type.CanPasteText == false) { App.CurrentVMHelp.ShowNegativeHelpbar("I can't paste text to a " + word.Type.Name + "."); return; }

            string currentContent = File.ReadAllText(word.FileInfo.FullName);
            string newContent;

            if (textAddMethod == TextAddMethod.Prepend)
            {
                text = text + Environment.NewLine + Environment.NewLine;
                newContent = text + currentContent;
            }
            else
            {
                text = Environment.NewLine + Environment.NewLine + text;
                newContent = currentContent + text;
            }

            File.WriteAllText(word.FileInfo.FullName, newContent);
            App.CurrentVMHelp.ShowPositiveHelpbar("I've added text from the clipboard to " + word.Name);
        }

        public void SaveTextToNote(string text, Word noteWord)
        {
            File.WriteAllText(noteWord.FileInfo.FullName, text);
        }
    }
}
