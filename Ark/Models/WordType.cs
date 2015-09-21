using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ark.Models
{
    public class WordType
    {
        public enum WordTypeEnum
        {
            Folder,
            Note
        }

        private WordTypeEnum thisType { get; set; }

        public bool CanPasteText
        {
            get
            {
                if (thisType == WordTypeEnum.Note) { return true; } else { return false; }
            }
        }

        public bool CanPasteFSO
        {
            get
            {
                if (thisType == WordTypeEnum.Folder) { return true; } else { return false; }
            }
        }

        public bool IsFile
        {
            get
            {
                if (thisType == WordTypeEnum.Folder) { return false; } else { return true; } 
            }
        }
        public bool IsFolder
        {
            get
            {
                if (thisType == WordTypeEnum.Folder) { return true; } else { return false; }
            }
        }
        public bool IsNote
        {
            get
            {
                if (thisType == WordTypeEnum.Note) { return true; } else { return false; }
            }
        }

        public WordType(WordTypeEnum type)
        {
            string rootFolder = Properties.Settings.Default.WordRootFolder;
            thisType = type;

            switch (thisType)
            {
                case WordTypeEnum.Folder:
                    TypeFolder = new DirectoryInfo(Path.Combine(rootFolder, "Folders"));
                    Extension = "";
                    IconPath = "icons/iconFolder.png";
                    break;
                case WordTypeEnum.Note:
                    TypeFolder = new DirectoryInfo(Path.Combine(rootFolder, "Notes"));
                    Extension = ".txt";
                    IconPath = "icons/iconNote.png";
                    break;
                default:
                    break;
            }
        }

        public string Name { get { return thisType.ToString().ToLower(); } }
        public DirectoryInfo TypeFolder { get; set; }
        public string TypeFolderBasepath { get { return TypeFolder.FullName; } }
        public string Extension { get; set; }
        public string IconPath { get; set; }
    }
}
