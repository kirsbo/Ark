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

        public DirectoryInfo MoveFolderToFolder(DirectoryInfo folder, string targetFolder)
        {
            moveFolder(folder.FullName, targetFolder);
            return new DirectoryInfo(targetFolder);
        }

        public DirectoryInfo MoveFolderToArchiveRoot(DirectoryInfo sourceFolder)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Archive);
            string archiveRoot = Properties.Settings.Default.ArchiveRootFolder;
            string newPath = Path.Combine(archiveRoot, sourceFolder.Name);
            moveFolder(sourceFolder.FullName, newPath);
            return new DirectoryInfo(newPath);
        }

        public void MoveFSOsToArchiveItem(List<string> pathsToSave, ArchiveItem archiveItem)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Archive);
            int folderMovedCount = 0; int fileMovedCount = 0;
            string destinationFolder = archiveItem.DirInfo.FullName; 

            foreach (string path in pathsToSave)
            {
                FileAttributes attr = File.GetAttributes(path);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    string newPath = Path.Combine(destinationFolder, di.Name);

                    moveFolder(path, newPath);
                    folderMovedCount++;
                }
                else
                {
                    FileInfo fi = new FileInfo(path);
                    string newPath = Path.Combine(destinationFolder, fi.Name);

                    moveFile(path, newPath);
                    fileMovedCount++;
                }
            }

            string confirmationMessage = "I've saved ";
            if (folderMovedCount > 0) { confirmationMessage = confirmationMessage + folderMovedCount + " folders"; }
            if (fileMovedCount > 0) { confirmationMessage = confirmationMessage + " " + fileMovedCount + " files"; }
            confirmationMessage = confirmationMessage + " to " + archiveItem.Name + ".";
            App.CurrentVMHelp.ShowPositiveHelpbar(confirmationMessage);

        }

        private void moveFolder(string sourceFolder, string targetFolder)
        {
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveDirectory(sourceFolder, targetFolder, true);
        }
        private void moveFile(string sourceFile, string targetPath)
        {
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.MoveFile(sourceFile, targetPath, true);
        }
    }
}
