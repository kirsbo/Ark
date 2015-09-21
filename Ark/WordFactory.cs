using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ark.Models;
using Ark.IO;

namespace Ark
{
    public class WordFactory
    {
        public List<Word> AllWords { 
            get {
                List<Word> allWords = new List<Word>();
                string rootFolder = Properties.Settings.Default.WordRootFolder;

                DirectoryInfo rootFolderDI = new DirectoryInfo(rootFolder);
                DirectoryInfo folderFolderDI = new DirectoryInfo(System.IO.Path.Combine(rootFolder, "Folders"));
                DirectoryInfo noteFolderDI = new DirectoryInfo(System.IO.Path.Combine(rootFolder, "Notes"));

                foreach (DirectoryInfo wordFolder in folderFolderDI.GetDirectories())
                {
                    allWords.Add(new Word(wordFolder));
                }

                foreach (FileInfo noteFile in noteFolderDI.GetFiles())
                {
                    allWords.Add(new Word(noteFile));
                }

                return allWords;
            } 
        }

        public WordFactory()
        {
            
        }

        public Word CreateNewWord(WordType wordType)
        {
            if (wordType.IsNote)
            {
                ioNoteCreator noteCreator = new ioNoteCreator();
                FileInfo fi = noteCreator.CreateNewNoteFile();
                Word word = new Word(fi);
                return word;
            }

            if (wordType.IsFolder)
            {
                ioFolderCreator folderCreator = new ioFolderCreator();
                DirectoryInfo di = folderCreator.CreateNewFolder();
                Word word = new Word(di);
                return word;
            }

            return null;
        }

        public Word CreateNoteWordFromFile(FileInfo noteFile)
        {
            ioNoteCreator noteCreator = new ioNoteCreator();
            FileInfo newNoteFile = noteCreator.CreateNoteFromFile(noteFile);
            return new Word(newNoteFile);
        }

        public Word CreateNoteWordFromString(string text)
        {
            ioNoteCreator noteCreator = new ioNoteCreator();
            FileInfo newNoteFile = noteCreator.CreateNewNoteFile(text);
            return new Word(newNoteFile);
        }

        public Word CreateFolderWordFromFolder(DirectoryInfo folderDI)
        {
            ioFolderCreator folderCreator = new ioFolderCreator();
            Word newWord = new Word(folderCreator.CreateFolderFromDI(folderDI));

            return newWord;
        }

        public Word CreateFolderWordFromList(List<string> fsoPaths)
        {
            Word newWord = CreateNewWord(new WordType(WordType.WordTypeEnum.Folder));

            ioFSOMover fsoMover = new ioFSOMover();
            fsoMover.MoveFSOsToWord(fsoPaths, newWord);

            return newWord;
        }
    }
}
