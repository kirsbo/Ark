using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ark.Models;

namespace Ark.Models
{
    public class ArchiveItem
    {
        public DirectoryInfo DirInfo; 

        public string IconPath { get { return "icons/iconFolder.png"; } }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFriendly { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public string LastAccessedDateFriendly { get; set; }

        public ArchiveItem(DirectoryInfo folder)
        {
            DirInfo = folder;
            Name = folder.Name;
            LastAccessedDate = folder.LastAccessTime;
            LastAccessedDateFriendly = LastAccessedDate.ToString("yyyy-MM-dd");
            CreatedDate = folder.CreationTime;
            CreatedDateFriendly = CreatedDate.ToString("yyyy-MM-dd");
            string parentFolder = DirInfo.Parent.Name.ToLower();
        }

    }
}
