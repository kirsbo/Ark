using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ark.Models;
using System.IO;

namespace Ark.IO
{
    class ioRenamer : ioObject
    {
        public bool AttemptRename(ArchiveItem archiveItem, string newName)
        {
            if (archiveItem.Name == newName) { return false; }
            Microsoft.VisualBasic.Devices.Computer pc = new Microsoft.VisualBasic.Devices.Computer();

            string newPath = Path.Combine(archiveItem.DirInfo.Parent.FullName, newName);
            if (Directory.Exists(newPath))
            {
                App.CurrentVMHelp.ShowNegativeHelpbar(String.Format("I can't rename to \"{0}\" as there is already a folder with that name.", newName));
                return false;
            }
            pc.FileSystem.RenameDirectory(archiveItem.DirInfo.FullName, newName);

            archiveItem.DirInfo = new DirectoryInfo(newPath);
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've renamed the folder {0} to {1}.", archiveItem.Name, newName));

            archiveItem.Name = newName;
            return true;
        }
    }
}
