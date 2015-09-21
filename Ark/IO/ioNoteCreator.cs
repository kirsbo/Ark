using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ark.Models;

namespace Ark.IO
{
    class ioNoteCreator : ioObject
    {

        public FileInfo CreateNoteFromFile(FileInfo noteFile)
        {
            string newNoteFileName = getNewNoteName(Path.GetFileNameWithoutExtension(noteFile.Name));
            string newNotePath = Path.Combine(noteFolderDI.FullName, newNoteFileName);

            ioFSOMover fileMover = new ioFSOMover();
            
            FileInfo fi = fileMover.MoveTextFileToNoteRoot(noteFile, newNotePath);
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've created a note named \"{0}\".", Path.GetFileNameWithoutExtension(fi.Name)));
            return fi;
        }

        public FileInfo CreateNewNoteFile(string text = "")
        {
            string newNoteFileName = getNewNoteName();
            string newNotePath = Path.Combine(noteFolderDI.FullName, newNoteFileName);
            FileInfo fi = new FileInfo(newNotePath);

            System.IO.File.WriteAllText(newNotePath, text, Encoding.UTF32);
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I've created a new note named \"{0}\".", Path.GetFileNameWithoutExtension(fi.Name)));
            
            return fi;
        }
     
        private string getNewNoteName(string baseFileName = "")
        {
            int counter = 2;
            
            if (baseFileName.Length == 0) { baseFileName = "Untitled note"; }

            string fullPath = Path.Combine(noteFolderDI.FullName, baseFileName + ".txt");
            if (File.Exists(fullPath) == false) { return fullPath; }

            fullPath = String.Format("{0} {1}{2}", Path.Combine(noteFolderDI.FullName, baseFileName), counter.ToString(), ".txt");
            string newFileName = fullPath;
            while (File.Exists(fullPath) == true) //Incrementing counter in new filename until we get a filename that doesn't exist. This means we'll get "Untitled [wordType] x" where x is a number.
            {
                newFileName = String.Format("{0} {1}{2}", baseFileName, counter.ToString(), ".txt");
                fullPath = Path.Combine(noteFolderDI.FullName, newFileName);
                counter++;
            }

            return newFileName;
        } //Finds the next new file name possible (ie. "Untitled note 2" or "Untitled note 3")

        
    }
}
