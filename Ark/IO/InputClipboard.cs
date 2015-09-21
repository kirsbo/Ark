using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ark.IO
{
    class InputClipboard : ioInput
    {
        public bool IsText { get; set; }
        public string ClipboardText { get; set; }

        public InputClipboard()
        {
            if (Clipboard.ContainsFileDropList())
            {
                StringCollection sc = Clipboard.GetFileDropList();
                List<string> fsoPaths = sc.Cast<string>().ToList();
                initFSOPath(fsoPaths);
            }

            if (Clipboard.ContainsText())
            {
                IsText = true;
                ClipboardText = Clipboard.GetText();
            }

            
        }
    }
}
