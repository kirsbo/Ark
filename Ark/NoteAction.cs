using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ark
{
    public class NoteAction
    {
        public string Name { get; set; }
        public string Action;
        public ActionTypeEnum ActionType;
        public int LineIndex;

        public enum ActionTypeEnum
        {
            Folder, File, AppShortcut, URL, TextSnippet, CheckboxFalse, CheckboxTrue, Nothing
        }

        public NoteAction()
        {

        }

    }
}
