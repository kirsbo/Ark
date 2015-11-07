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


        protected string getNewFolderName(string folderName = "", DirectoryInfo targetFolder = null)
        {
            int counter = 2;

            if (folderName.Length == 0) { folderName = "Untitled folder"; }
            if (targetFolder== null) { targetFolder = rootFolderDI; }

            string newFolderName = null;

            string fullPath = Path.Combine(targetFolder.FullName, folderName);
            if (Directory.Exists(fullPath) == false) { return fullPath; }

            while (Directory.Exists(fullPath) == true) //Incrementing counter in new folder name until we get a folder name that doesn't exist. This means we'll get "Untitled folder x" where x i a number.
            {
                newFolderName = String.Format("{0} {1}", folderName, counter.ToString());
                fullPath = Path.Combine(targetFolder.FullName, newFolderName);
                counter++;
            }

            return newFolderName;
        } //Finds the next new folder name possible (ie. "Untitled folder 2" etc)

        protected string getNewFileName(string fileName, DirectoryInfo targetFolder)
        {
            int counter = 2;
            string newFileName = null;
            
            string filePath = Path.Combine(targetFolder.FullName, fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);

            if (File.Exists(filePath) == false) { return filePath; }

            while (File.Exists(filePath) == true) //Incrementing counter in new folder name until we get a folder name that doesn't exist. This means we'll get "Untitled folder x" where x i a number.
            {
                newFileName = String.Format("{0} {1}{2}", fileNameNoExt, counter.ToString(), extension);
                filePath = Path.Combine(targetFolder.FullName, newFileName);
                counter++;
            }

            return newFileName;
        } //Finds the next new file name possible (in case we're trying to move files to a location where a file with the same name already exists)

    }
}
