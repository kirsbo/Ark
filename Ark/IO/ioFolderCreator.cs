using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ark.IO
{
    class ioFolderCreator : ioObject
    {

        public DirectoryInfo CreateNewFolder()
        {
            string folderName = getNewFolderName();
            string newFolderPath = Path.Combine(base.rootFolderDI.FullName, folderName);
            DirectoryInfo di = new DirectoryInfo(newFolderPath);

            Directory.CreateDirectory(newFolderPath);
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've created a folder named \"{0}\".", folderName));

            return di;
        }


    }
}
