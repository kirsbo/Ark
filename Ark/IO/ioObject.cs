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

        public ioObject()
        {
            string rootFolder = Properties.Settings.Default.ArchiveRootFolder;

            rootFolderDI = new DirectoryInfo(rootFolder);
        }

    }
}
