using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Ark.IO
{
    public class InputDragDrop : ioInput
    {
        public bool ObjectHasDataPresent { get; set; }
        
        public InputDragDrop(DragEventArgs e)
        {
            ObjectHasDataPresent = e.Data.GetDataPresent(DataFormats.FileDrop);
            List<string> fsoPaths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();

            initFSOPath(fsoPaths);
        }

    }
}
