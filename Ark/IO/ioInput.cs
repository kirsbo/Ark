using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace Ark.IO
{
    public abstract class ioInput : INotifyPropertyChanged
    {
        public bool IsSingleFile { get; set; }
        private bool isSingleFolder;
        public bool IsSingleFolder
        {
            get { return isSingleFolder; }
            set
            {
                isSingleFolder = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSingleFolder"));
            }
        }

        public bool IsMultipleFSOs { get; set; }
        public bool IsFSO { get; set; }
        private int itemCount;
        public int ItemCount
        {
            get { return itemCount; }
            set
            {
                itemCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ItemCount"));
            }
        }

        private List<string> fsoPaths;
        public List<string> FSOPaths
        {
            get { return fsoPaths; }
            set
            {
                fsoPaths = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FSOPaths"));
            }
        }
        private List<string> fsoNamesSorted;
        public List<string> FSONamesSorted
        {
            get { return fsoNamesSorted; }
            set
            {
                fsoNamesSorted = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FSONamesSorted"));
            }
        }

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
            IsMultipleFSOs = false;
            IsFSO = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public void AddPaths(List<string> fsoPaths)
        {
            FSOPaths.AddRange(fsoPaths);
            initFSOPath(FSOPaths);
        }

        public void RemovePath(string fsoPath)
        {
            FSOPaths.Remove(fsoPath);
            initFSOPath(FSOPaths);
        }

        protected void initFSOPath(List<string> fsoPaths)
        {
            IsFSO = true;
            FSOPaths = fsoPaths;

            FSONamesSorted = getSortedFSONames(FSOPaths);

            if (FSOPaths.Count == 1)
            {
                if (isFile(FirstFSO))
                {
                    IsSingleFile = true;
                }
                else
                {
                    IsSingleFolder = true;
                }
            }
            else if (FSOPaths.Count > 1) { IsMultipleFSOs = true; IsSingleFolder = false;  IsSingleFile = false; }

            ItemCount = FSOPaths.Count;
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


        private List<string> getSortedFSONames(List<string> fsoPaths)
        {
            List<string> sortedFSONames = new List<string>();
            foreach (string path in fsoPaths)
            {
                FileAttributes attr = File.GetAttributes(path);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    sortedFSONames.Add(di.FullName);
                }
                else
                {
                    FileInfo fi = new FileInfo(path);
                    sortedFSONames.Add(fi.FullName);
                }
            }

            sortedFSONames = sortedFSONames.OrderBy(x => x).ToList();
            return sortedFSONames;
        }
    }
}
