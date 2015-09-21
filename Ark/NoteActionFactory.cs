using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Ark
{
    public class NoteActionFactory
    {
        public List<NoteAction> NoteActions { get; set; }


        public NoteActionFactory(string text)
        {
            string[] allLines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<NoteAction> noteActions = new List<NoteAction>();

            int counter = 0;
            foreach (string line in allLines)
            {
                NoteAction newNoteAction = getNoteActionFromLine(line, counter);
                if (newNoteAction.ActionType != NoteAction.ActionTypeEnum.Nothing)
                {
                    noteActions.Add(newNoteAction);
                }
                counter++;
            }

            NoteActions = noteActions;
        }


        private NoteAction getNoteActionFromLine(string line, int lineIndex)
        {
            bool isDefinedAction = false;
            NoteAction noteAction = new NoteAction();
            noteAction.LineIndex = lineIndex;

            string name = "";

            if (line.StartsWith("()")) 
            {
                noteAction.ActionType = NoteAction.ActionTypeEnum.CheckboxFalse;
                noteAction.Name = line.Substring(2).Trim();
                return noteAction;
            }

            if (line.StartsWith("( )"))
            {
                noteAction.ActionType = NoteAction.ActionTypeEnum.CheckboxFalse;
                noteAction.Name = line.Substring(3).Trim();
                return noteAction;
            }

            if (line.ToLower().StartsWith("(x)")) {
                noteAction.ActionType = NoteAction.ActionTypeEnum.CheckboxTrue;
                noteAction.Name = line.Substring(3).Trim();
                return noteAction;

            }

            if (line.Contains("::")) //A defined note action is an action the user has written into the note. It is in the format [noteName]::[noteAction]
            {
                int separatorIndex = line.IndexOf("::"); //Separator char is ::
                name = line.Substring(0, separatorIndex);
                line = line.Substring(separatorIndex + 2);
                isDefinedAction = true;
            }

            KeyValuePair<NoteAction.ActionTypeEnum, string> action = getActionFromLine(line, isDefinedAction);
            
            noteAction.ActionType = action.Key;
            noteAction.Action = action.Value;
            if (name == "") { 
                noteAction.Name = action.Value; 
            }
            else { noteAction.Name = name; }

            return noteAction;
        }

        private KeyValuePair<NoteAction.ActionTypeEnum, string> getActionFromLine(string line, bool isDefinedAction)
        {
            NoteAction.ActionTypeEnum kpKey;
            string kpValue = "";

            Regex regex; Match match;

            //URL check
            regex = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");
            match = regex.Match(line);
            if (match.Success)
            {
                Uri uri = new Uri(match.Value);
                kpKey = NoteAction.ActionTypeEnum.URL;
                kpValue = uri.AbsoluteUri.ToString();
                return new KeyValuePair<NoteAction.ActionTypeEnum, string>(kpKey, kpValue);
            }

            //Path check
            regex = new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
            match = regex.Match(line);
            if (match.Success)
            {
                if (File.Exists(match.Value))
                {
                    kpKey = NoteAction.ActionTypeEnum.File;
                    FileInfo fi = new FileInfo(match.Value);
                    if (fi.Extension.ToLower() == ".exe") { kpKey = NoteAction.ActionTypeEnum.AppShortcut; }
                    kpValue = fi.FullName;

                    return new KeyValuePair<NoteAction.ActionTypeEnum, string>(kpKey, kpValue);
                }
                else if (Directory.Exists(match.Value))
                {
                    kpKey = NoteAction.ActionTypeEnum.Folder;
                    DirectoryInfo di = new DirectoryInfo(match.Value);
                    kpValue = di.FullName;

                    return new KeyValuePair<NoteAction.ActionTypeEnum, string>(kpKey, kpValue);
                }
            }

            //Snippet check
            if (isDefinedAction)
            {
                kpKey = NoteAction.ActionTypeEnum.TextSnippet;
                kpValue = line;
                return new KeyValuePair<NoteAction.ActionTypeEnum, string>(kpKey, kpValue);
            }

            return new KeyValuePair<NoteAction.ActionTypeEnum, string>(NoteAction.ActionTypeEnum.Nothing,"");
        }
    }
}
