using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ark.Models;

namespace Ark.IO
{
    class ioFSOMover : ioObject
    {
        public void MoveFSOsToArchive(List<string> pathsToSave, DirectoryInfo targetFolder)
        {
            int folderMovedCount = 0; int fileMovedCount = 0;

            foreach (string path in pathsToSave)
            {
                FileAttributes attr = File.GetAttributes(path);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    
                    string folderName = getNewFolderName(di.Name, targetFolder);
                    string newPath = Path.Combine(targetFolder.FullName, folderName);

                    moveFolder(path, newPath);
                    folderMovedCount++;
                }
                else
                {
                    FileInfo fi = new FileInfo(path);

                    string fileName = getNewFileName(fi.Name, targetFolder);
                    string newPath = Path.Combine(targetFolder.FullName, fileName);

                    moveFile(path, newPath);
                    fileMovedCount++;
                }
            }

            App.CurrentVMHelp.ShowPositiveHelpbar(getConfirmationMessage(folderMovedCount, fileMovedCount, targetFolder.Name));
        } // Moves FSO's to either a specified folder or the root folder if no specific folder is supplied.


        #region Supporting methods
        
        private string getConfirmationMessage(int folderCount, int fileCount, string targetFolderName)
        {
            string confirmationMessage = "I've saved ";
            if (folderCount > 0) { confirmationMessage = confirmationMessage + folderCount + " folders"; }
            if (fileCount > 0) { confirmationMessage = confirmationMessage + " " + fileCount + " files"; }
            confirmationMessage = confirmationMessage + " to " + targetFolderName + ".";

            return confirmationMessage;
        }

        private void moveFolder(string sourceFolder, string targetFolder)
        {
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(sourceFolder, targetFolder, false);
        }
        private void moveFile(string sourceFile, string targetPath)
        {
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveFile(sourceFile, targetPath, false);
        }

        #endregion
    }
}
