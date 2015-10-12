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

        public DirectoryInfo CreateNamedFolder(string folderName)
        {
            string newFolderPath = Path.Combine(base.rootFolderDI.FullName, folderName);
            if (Directory.Exists(newFolderPath))
            {
                App.CurrentVMHelp.ShowNegativeHelpbar("I can't create a folder called " + folderName + " as it already exists.");
                return null;
            }
            else
            {
                Directory.CreateDirectory(newFolderPath);
                DirectoryInfo di = new DirectoryInfo(newFolderPath);
                App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've created a folder named \"{0}\".", folderName));
                return di;
            }
        }

        public DirectoryInfo CreateFolderFromDI(DirectoryInfo folder)
        {
            string newFolderName = getNewFolderName(folder.Name);
            string newFolderPath = Path.Combine(base.rootFolderDI.FullName, newFolderName);

            ioFSOMover fsoMover = new ioFSOMover();
            DirectoryInfo di = fsoMover.MoveFolderToFolder(folder, newFolderPath);
            
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've saved the folder named \"{0}\".", di.Name));

            return di;
        }

        public DirectoryInfo CreateNewFolder()
        {
            string folderName = getNewFolderName();
            string newFolderPath = Path.Combine(base.rootFolderDI.FullName, folderName);
            DirectoryInfo di = new DirectoryInfo(newFolderPath);

            Directory.CreateDirectory(newFolderPath);
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've created a folder named \"{0}\".", folderName));

            return di;
        }


        private string getNewFolderName(string baseFolderName = "")
        {
            int counter = 2;

            if (baseFolderName.Length == 0) { baseFolderName = "Untitled folder"; }
            string newFolderName = null;

            string fullPath = Path.Combine(base.rootFolderDI.FullName, baseFolderName);
            if (Directory.Exists(fullPath) == false) { return fullPath; }

            while (Directory.Exists(fullPath) == true) //Incrementing counter in new folder name until we get a folder name that doesn't exist. This means we'll get "Untitled folder x" where x i a number.
            {
                newFolderName = String.Format("{0} {1}", baseFolderName, counter.ToString());
                fullPath = Path.Combine(base.rootFolderDI.FullName, newFolderName);
                counter++;
            }

            return newFolderName;
        } //Finds the next new folder name possible (ie. "Untitled folder 2" etc)
    }
}
