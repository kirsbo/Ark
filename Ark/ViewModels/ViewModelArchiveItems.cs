using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ark.Models;
using Ark.IO;
using System.IO;


namespace Ark.ViewModels
{
    public class ViewModelArchiveItems : DependencyObject
    {
        private List<ArchiveItem> allArchiveItems;
        private DirectoryInfo rootFolder;
 
        public ViewModelArchiveItems()
        {
            rootFolder = new DirectoryInfo(Properties.Settings.Default.ArchiveRootFolder);
            ArchiveItemFactory archiveItemFactory = new ArchiveItemFactory();
            allArchiveItems = archiveItemFactory.AllArchiveItems;
            FilteredWords = allArchiveItems;
        }

        public List<ArchiveItem> FilteredWords
        {
            get { return (List<ArchiveItem>)GetValue(FilteredWordsProperty); }
            set { SetValue(FilteredWordsProperty, value); }
        }
        public static readonly DependencyProperty FilteredWordsProperty = DependencyProperty.Register("FilteredWords", typeof(List<ArchiveItem>), typeof(ViewModelArchiveItems), new PropertyMetadata(default(ArchiveItem)));

        public string SearchFilter
        {
            get { return (string)GetValue(SearchFilterProperty); }
            set { SetValue(SearchFilterProperty, value); }
        }
        public static readonly DependencyProperty SearchFilterProperty = DependencyProperty.Register("SearchFilter", typeof(string), typeof(ViewModelArchiveItems), new UIPropertyMetadata(searchFilterChangedHandler));

        public bool UserIsRenamingFolder
        {
            get { return (bool)GetValue(UserIsRenamingFolderProperty); }
            set { SetValue(UserIsRenamingFolderProperty, value); }
        }
        public static readonly DependencyProperty UserIsRenamingFolderProperty = DependencyProperty.Register("UserIsRenamingFolder", typeof(bool), typeof(ViewModelArchiveItems), new PropertyMetadata(default(bool)));


        #region Results listbox filtering
        public static void searchFilterChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModelArchiveItems vmRef = (ViewModelArchiveItems)sender;
            vmRef.OnSearchFilterChanged(e);
        }

        private void OnSearchFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            filterWords(SearchFilter);
        }
        #endregion



        private void filterWords(string filter)
        {
            if (filter.Length == 0) { 
                FilteredWords = allArchiveItems; return; 
            }

            List<string> filterWords = filter.Split(' ').ToList();
            List<ArchiveItem> filteredWords = allArchiveItems.Where(a => filterWords.All(b => a.Name.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            //Making sure an exact match is returned as first item. This is to avoid searching for "Untitled folder" and then "Untitled folder 2" is shown before "Untitled folder" in the results list, due to alphabetical sorting.
            ArchiveItem exactMatch = filteredWords.Find(w => w.Name.ToLower() == filter.ToLower());
            if (exactMatch != null)
            {
                int matchIndex = filteredWords.FindIndex(w => w.Name.ToLower() == filter.ToLower());
                if (matchIndex > 0)
                {
                    filteredWords.Swap(0, matchIndex);
                }
            }

            FilteredWords = filteredWords;
        }

        public void CreateNewFolder()
        {
            ioFolderCreator creator = new ioFolderCreator();
            ArchiveItem newFolder = new ArchiveItem(creator.CreateNewFolder());
            allArchiveItems.Add(newFolder);

            SearchFilter = newFolder.Name;
            UserIsRenamingFolder = true;
        }

        public void DeleteFolder(ArchiveItem itemToBeDeleted)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(itemToBeDeleted.DirInfo.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            allArchiveItems.Remove(itemToBeDeleted);
            SearchFilter = " ";
            SearchFilter = "";
            App.CurrentVMHelp.ShowPositiveHelpbar(String.Format("I have deleted {0}. It can be recovered from the Windows recycle bin.", itemToBeDeleted.Name));
        }

        public void ArchiveInput(ioInput input, DirectoryInfo targetFolder = null)
        {
            bool archivingWithSameName = false;
            if (targetFolder == null) {
                targetFolder = rootFolder;
                archivingWithSameName = true; //Handling the edge case where user clicked "Archive with same name", i.e. Creating a new folder in the root with the same name as the folder being archived (basically just moving a folder from somewhere to ark root).
            }
            archiveItems(input.FSOPaths, targetFolder);

            string searchFilter = "";
            if (archivingWithSameName)
            {
                DirectoryInfo di = new DirectoryInfo(input.FSOPaths[0]);
                
                string sameNameNewPath = System.IO.Path.Combine(rootFolder.FullName, di.Name);
                DirectoryInfo sameNameNewPathDI = new DirectoryInfo(sameNameNewPath);
                allArchiveItems.Add(new ArchiveItem(sameNameNewPathDI));
                searchFilter = sameNameNewPathDI.Name;
            }
            else
            {
                searchFilter = targetFolder.Name;
            }

            SearchFilter = searchFilter; 
        }

        private void archiveItems(List<string> paths, DirectoryInfo targetFolder)
        {
            ioFSOMover mover = new ioFSOMover();
            mover.MoveFSOsToArchive(paths, targetFolder);

            
            SoundEffects.Play(SoundEffects.EffectEnum.Archive);
            if (Properties.Settings.Default.CloseAfterArchiving) { Application.Current.Shutdown(); }
        }

    }
}
