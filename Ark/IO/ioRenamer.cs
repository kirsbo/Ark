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
        public bool AttemptRename(Word word, string newName)
        {
            if (word.Name == newName) { return false; }
            Microsoft.VisualBasic.Devices.Computer pc = new Microsoft.VisualBasic.Devices.Computer();

            if (word.Type.IsFolder)
            {
                string newPath = Path.Combine(word.DirInfo.Parent.FullName, newName);
                if (Directory.Exists(newPath))
                {
                    App.CurrentVMHelp.ShowNegativeHelpbar(String.Format("I can't rename to \"{1}\" as there is already a {0} with that name.", word.Type.Name, newName));
                    return false;
                }
                pc.FileSystem.RenameDirectory(word.DirInfo.FullName, newName);

                word.DirInfo = new DirectoryInfo(newPath);
                App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've renamed the folder {0} to {1}.", word.Name, newName));
            }
            else
            {
                string extension = word.FileInfo.Extension;
                string newFileName = newName + extension;
                pc.FileSystem.RenameFile(word.FileInfo.FullName, newFileName);
                string newPath = Path.Combine(word.FileInfo.DirectoryName, newFileName);
                word.FileInfo = new FileInfo(newPath);

                App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've renamed the {0} {1} to {2}.", word.Type.Name, word.Name, newName));
            }

            word.Name = newName;
            return true;
        }
    }
}
