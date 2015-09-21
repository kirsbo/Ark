using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ark.Models;
using Ark.IO;
using System.Text.RegularExpressions;
using System.IO;

namespace Ark.ViewModels
{
    public class ViewModelNotePage : DependencyObject
    {
        public List<NoteAction> NoteActions
        {
            get { return (List<NoteAction>)GetValue(NoteActionsProperty); }
            set { SetValue(NoteActionsProperty, value); }
        }
        public static readonly DependencyProperty NoteActionsProperty = DependencyProperty.Register("NoteActions", typeof(List<NoteAction>), typeof(ViewModelNotePage));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ViewModelNotePage));

        public Word NoteWord
        {
            get { return (Word)GetValue(NoteWordProperty); }
            set { SetValue(NoteWordProperty, value); }
        }
        public static readonly DependencyProperty NoteWordProperty = DependencyProperty.Register("NoteWord", typeof(Word), typeof(ViewModelNotePage));

        public ViewModelNotePage(Word noteWord) {
            NoteWord = noteWord;
            Text = File.ReadAllText(noteWord.FileInfo.FullName);
            TextChange(Text);
        }

        public void TextChange(string text)
        {
            Text = text;
            ioNoteTextSaver textSaver = new ioNoteTextSaver();
            textSaver.SaveTextToNote(Text, NoteWord);

            NoteActions = new NoteActionFactory(text).NoteActions; 
        }


    }
}
