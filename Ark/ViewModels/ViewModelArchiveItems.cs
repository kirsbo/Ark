using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ark.Models;
using Ark.IO;


namespace Ark.ViewModels
{
    public class ViewModelArchiveItems : DependencyObject
    {
        private List<ArchiveItem> allArchiveItems;
        private ArchiveItemFactory archiveItemFactory;
 
        public ViewModelArchiveItems()
        {
            archiveItemFactory = new ArchiveItemFactory();
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
            if (filter.Length == 0) { FilteredWords = allArchiveItems; return; }

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



    }
}
