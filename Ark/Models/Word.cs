using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ark.Models;

namespace Ark.Models
{
    public class Word
    {
        public WordType Type { get; set; }

        public DirectoryInfo DirInfo; //DirInfo is used only for words of type "folder"
        public FileInfo FileInfo;  //Fileinfo is used only for words of type "note"

        public string IconPath { get { return Type.IconPath; } }

        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFriendly { get; set; }

        public Word(FileInfo file = null)
        {
            FileInfo = file;
            Name = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
            CreatedDate = file.CreationTime;
            CreatedDateFriendly = CreatedDate.ToString("yyyy-MM-dd");

            string fileExtension = file.Extension.ToLower();
            switch (fileExtension)
            {
                case ".txt":
                    Type = new WordType(Models.WordType.WordTypeEnum.Note); 
                    break;
            }
        }


        public Word(DirectoryInfo folder = null)
        {
            DirInfo = folder;
            Name = folder.Name;
            CreatedDate = folder.CreationTime;
            CreatedDateFriendly = CreatedDate.ToString("yyyy-MM-dd");
            Type = new WordType(Models.WordType.WordTypeEnum.Folder); 
            string parentFolder = DirInfo.Parent.Name.ToLower();
        }

    }
}
