using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Ark.Models;
using Ark.ViewModels;

namespace Ark
{
    /// <summary>
    /// Interaction logic for pageNote.xaml
    /// </summary>
    public partial class pageNote : Page
    {
        private ViewModelNotePage vmNotePage;

        public pageNote(Word noteWord)
        {
            InitializeComponent();

            vmNotePage = new ViewModelNotePage(noteWord);
            this.DataContext = vmNotePage;
        }

        private void txtNote_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                if ((Keyboard.Modifiers == ModifierKeys.Control) && (Keyboard.Modifiers == ModifierKeys.Alt))
                {
                    var insertText = "()";
                    var selectionIndex = txtNote.SelectionStart;
                    txtNote.Text = txtNote.Text.Insert(selectionIndex, insertText);
                    txtNote.SelectionStart = selectionIndex + insertText.Length;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.GlobalNavigator.Navigate(new pageSearch());
        }

        private void txtNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            vmNotePage.TextChange(txtNote.Text);
        }

        private void noteAction_checkboxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                checkboxValueChanged((NoteAction)listNoteActions.SelectedItem);
            }
        }

        private void checkboxValueChanged(NoteAction noteAction)
        {

            switch (noteAction.ActionType)
            {
                case NoteAction.ActionTypeEnum.Folder:
                case NoteAction.ActionTypeEnum.File:
                case NoteAction.ActionTypeEnum.AppShortcut:
                case NoteAction.ActionTypeEnum.URL:
                    System.Diagnostics.Process.Start(noteAction.Action);
                    break;
                case NoteAction.ActionTypeEnum.TextSnippet:
                    Clipboard.SetText(noteAction.Action);
                    break;
                case NoteAction.ActionTypeEnum.CheckboxFalse:
                case NoteAction.ActionTypeEnum.CheckboxTrue:
                    string[] lines = txtNote.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    lines[noteAction.LineIndex] = toggleCheckboxString(lines[noteAction.LineIndex]);
                    txtNote.Text = String.Join(Environment.NewLine, lines);
                    break;
                case NoteAction.ActionTypeEnum.Nothing:
                    break;
                default:
                    break;
            }
        }

        private string toggleCheckboxString(string line)
        {
            if (line.StartsWith("()"))
            {
                return line.Replace("()", "(X)");
            }
            if (line.StartsWith("( )"))
            {
                return line.Replace("( )", "(X)");
            }
            if ((line.StartsWith("(x)")) || (line.StartsWith("(X)")))
            {
                string stringToReplace = line.Replace("(x)", "()");
                stringToReplace = stringToReplace.Replace("(X)", "()");
                return stringToReplace;
            }

            return null;
        }

 
        private void CheckBox_CheckedChange(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            NoteAction noteAction = (NoteAction)cb.DataContext;
            checkboxValueChanged(noteAction);
        }

    
    }
}
