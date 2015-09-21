using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ark.Models;
using System.IO;

namespace Ark.ViewModels
{
    public class ViewModelSearchPage : DependencyObject
    {
        private List<Word> allWords;
        public WordFactory WordFactory;

        public void CreateAndAddNewWord(WordType wordType)
        {
            addNewWord(WordFactory.CreateNewWord(wordType));
        }

        public void CreateAndAddNoteFromFile(FileInfo noteFile)
        {
            Word newWord = WordFactory.CreateNoteWordFromFile(noteFile);
            addNewWord(newWord);
        }

        public void CreateAndAddNoteFromString(string text)
        {
            addNewWord(WordFactory.CreateNoteWordFromString(text));
        }

        public void CreateAndAddFolderFromFolder(DirectoryInfo folder)
        {
            addNewWord(WordFactory.CreateFolderWordFromFolder(folder));
        }

        public void CreateAndAddFolderFromList(List<string> fsoPaths)
        {
            addNewWord(WordFactory.CreateFolderWordFromList(fsoPaths));
        }

        private void addNewWord(Word word)
        {
            allWords.Add(word);
            Console.WriteLine("Setting filter in vm.");
            
            SearchFilter = word.Name;
            Console.WriteLine("Set filter in vm.");
            Console.WriteLine("Setting IsRenaming to true.");
            UserIsRenamingWord = true;
            Console.WriteLine("Set IsRenaming to true.");
        }




        #region Dependency properties



        public bool UserDraggedFSOsOnDesktopIcon
        {
            get { return (bool)GetValue(UserDraggedFSOsOnDesktopIconProperty); }
            set { SetValue(UserDraggedFSOsOnDesktopIconProperty, value); }
        }
        public static readonly DependencyProperty UserDraggedFSOsOnDesktopIconProperty = DependencyProperty.Register("UserDraggedFSOsOnDesktopIcon", typeof(bool), typeof(ViewModelSearchPage), new PropertyMetadata(false));

        public bool UserIsDraggingNote
        {
            get { return (bool)GetValue(UserIsDraggingNoteProperty); }
            set { SetValue(UserIsDraggingNoteProperty, value); }
        }
        public static readonly DependencyProperty UserIsDraggingNoteProperty = DependencyProperty.Register("UserIsDraggingNote", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(false));


        

        public bool UserIsDraggingOnDragDropPanel
        {
            get { return (bool)GetValue(UserIsDraggingOnDragDropPanelProperty); }
            set { SetValue(UserIsDraggingOnDragDropPanelProperty, value); }
        }
        public static readonly DependencyProperty UserIsDraggingOnDragDropPanelProperty = DependencyProperty.Register("UserIsDraggingOnDragDropPanel", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(false));

        public bool UserIsDragging
        {
            get { return (bool)GetValue(UserIsDraggingProperty); }
            set { SetValue(UserIsDraggingProperty, value); }
        }
        public static readonly DependencyProperty UserIsDraggingProperty = DependencyProperty.Register("UserIsDragging", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(false));
        

        public bool UserIsNotSearching
        {
            get { return (bool)GetValue(UserIsNotSearchingProperty); }
            set { SetValue(UserIsNotSearchingProperty, value); }
        }
        public static readonly DependencyProperty UserIsNotSearchingProperty = DependencyProperty.Register("UserIsNotSearching", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(true));

        public bool UserIsSearching
        {
            get { return (bool)GetValue(UserIsSearchingProperty); }
            set { SetValue(UserIsSearchingProperty, value); }
        }
        public static readonly DependencyProperty UserIsSearchingProperty = DependencyProperty.Register("UserIsSearching", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(true));

        public List<Word> FilteredWords
        {
            get { return (List<Word>)GetValue(FilteredWordsProperty); }
            set { SetValue(FilteredWordsProperty, value); }
        }
        public static readonly DependencyProperty FilteredWordsProperty = DependencyProperty.Register("FilteredWords", typeof(List<Word>), typeof(ViewModelSearchPage), new PropertyMetadata(default(Word)));

        public string SearchFilter
        {
            get { return (string)GetValue(SearchFilterProperty); }
            set { SetValue(SearchFilterProperty, value); }
        }
        public static readonly DependencyProperty SearchFilterProperty = DependencyProperty.Register("SearchFilter", typeof(string), typeof(ViewModelSearchPage), new UIPropertyMetadata(searchFilterChangedHandler));
        #endregion

        #region Results listbox filtering
        public static void searchFilterChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModelSearchPage vmRef = (ViewModelSearchPage)sender;
            vmRef.OnSearchFilterChanged(e);
        }

        private void OnSearchFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            filterWords(SearchFilter);
        }
        #endregion

        public bool UserIsRenamingWord
        {
            get { return (bool)GetValue(UserIsRenamingWordProperty); }
            set { SetValue(UserIsRenamingWordProperty, value); 
                Console.WriteLine("*** Is renaming: " + value.ToString());
            }
        }
        public static readonly DependencyProperty UserIsRenamingWordProperty = DependencyProperty.Register("UserIsRenamingWord", typeof(bool), typeof(ViewModelSearchPage), new PropertyMetadata(default(bool)));



        public ViewModelSearchPage(ViewModelHelp vmHelp)
        {
            WordFactory = new WordFactory();

            allWords = WordFactory.AllWords;
            FilteredWords = new List<Word>(); // WordFactory.AllWords;
            App.CurrentVMHelp = vmHelp;




            //## Setting up watcher -- 
            /*This needs to be refactored out into a watcher class that's referenced from this view model. This allows us to automatically update the allwords collection depending on folder 
             * and txt file events like creation, deletion, renaming etc. This allows us to get rid of a whole lot of code, since now the allwords collection will just reflect the filesystem. 
             * i.e. we do not need to create words from the view model through the io objects. Instead we can just create/delete/rename the underlying FSO objects. Meaning we can do it from any 
             * window / page without caring:
             * The words collection will always be up to date (as it's either initialized from scratch or updated by the FSO watcher class which is kept alive from the vmSearchPage.
             * All the "CreateAndXXXX" methods in this class can be removed for example, and from pageAddToNew or pageAddToExisting, we can easily create objects. We can also simplify and clean
             * out the ioobjects, so they are real simple objects.
             * 
             * 
             * 
             * */
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = @"D:\Dropbox\Code\ArkCabinet\Folders\";
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Add event handlers.
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;



        }

        #region TESTFSOWatcher
        
        // Define the event handlers. 
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        #endregion

        private void filterWords(string filter)
        {
            if (filter.Length == 0) { FilteredWords = allWords; return; }

            List<string> filterWords = filter.Split(' ').ToList();
            List<Word> filteredWords = allWords.Where(a => filterWords.All(b => a.Name.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            //Making sure an exact match is returned as first item. This is to avoid searching for "Untitled note" and then "Untitled note 2" is shown before "Untitled note" in the results list, due to alphabetical sorting.
            Word exactMatch = filteredWords.Find(w => w.Name.ToLower() == filter.ToLower());
            if (exactMatch != null) 
            {
                int matchIndex = filteredWords.FindIndex(w => w.Name.ToLower() == filter.ToLower());
                if (matchIndex > 0) {
                    filteredWords.Swap(0,matchIndex);
                }
            }

            FilteredWords = filteredWords;
        }

        public void OpenWord(Word word)
        {
            if (word.Type.IsFolder) { System.Diagnostics.Process.Start(word.DirInfo.FullName); }
            if (word.Type.IsNote) { App.GlobalNavigator.Navigate(new pageNote(word)); }
        }

        
    }
}
