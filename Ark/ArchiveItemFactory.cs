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
    public class ArchiveItemFactory
    {
        public List<ArchiveItem> AllArchiveItems
        {
            get
            {
                List<ArchiveItem> allArchiveItems = new List<ArchiveItem>();
                string rootFolder = Properties.Settings.Default.ArchiveRootFolder;

                DirectoryInfo rootFolderDI = new DirectoryInfo(rootFolder);

                foreach (DirectoryInfo folder in rootFolderDI.GetDirectories())
                {
                    allArchiveItems.Add(new ArchiveItem(folder));
                }

                return allArchiveItems.OrderBy(x => x.CreatedDate).ToList();
            }
        }

        public ArchiveItemFactory()
        {

        }

        public ArchiveItem CreateUntitledArchiveItem()
        {
            ioFolderCreator folderCreator = new ioFolderCreator();
            DirectoryInfo di = folderCreator.CreateNewFolder();
            ArchiveItem word = new ArchiveItem(di);
            return word;
        }


        public ArchiveItem CreateArchiveItemFromFolder(DirectoryInfo folderDI)
        {
            ioFolderCreator folderCreator = new ioFolderCreator();
            ArchiveItem newWord = new ArchiveItem(folderCreator.CreateFolderFromDI(folderDI));

            return newWord;
        }


    }
}
