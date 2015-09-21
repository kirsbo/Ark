using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ark.IO
{
    public class ioInput
    {
        public bool IsSingleFile { get; set; }
        public bool IsSingleFolder { get; set; }
        public bool IsSingleNoteFile { get; set; }
        public bool IsMultipleFSOs { get; set; }
        public bool IsFSO { get; set; }

        public List<string> FSOPaths { get; set; }

        public string FirstFSO
        {
            get { return FSOPaths[0]; }
        }
        public DirectoryInfo SingleFolderDI
        {
            get
            {
                if (IsSingleFolder) { return new DirectoryInfo(FirstFSO); }
                else { return null; }
            }
        }
        public FileInfo SingleFileDI
        {
            get
            {
                if (IsSingleFile) { return new FileInfo(FirstFSO); }
                else { return null; }
            }
        }


        public ioInput()
        {
            IsSingleFile = false;
            IsSingleFolder = false;
            IsSingleNoteFile = false;
            IsMultipleFSOs = false;
            IsFSO = false;
        }

        protected void initFSOPath(List<string> fsoPaths)
        {
            IsFSO = true;
            FSOPaths = fsoPaths;
            if (FSOPaths.Count == 1)
            {
                if (isFile(FirstFSO))
                {
                    IsSingleFile = true;
                    if (FirstFSO.ToLower().EndsWith(".txt")) { IsSingleNoteFile = true; }
                }
                else
                {
                    IsSingleFolder = true;
                }
            }
            else if (FSOPaths.Count > 1) { IsMultipleFSOs = true; }
        }


        private bool isFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
