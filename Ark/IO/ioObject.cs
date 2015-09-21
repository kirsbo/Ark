using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ark.IO
{
    class ioObject
    {
        protected DirectoryInfo rootFolderDI;
        protected DirectoryInfo folderFolderDI;
        protected DirectoryInfo noteFolderDI;

        public ioObject()
        {
            string rootFolder = Properties.Settings.Default.WordRootFolder;

            rootFolderDI = new DirectoryInfo(rootFolder);
            folderFolderDI = new DirectoryInfo(Path.Combine(rootFolder, "Folders"));
            noteFolderDI = new DirectoryInfo(Path.Combine(rootFolder, "Notes"));
        }

    }
}
