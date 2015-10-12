using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ark.Models;
using Ark.IO;
using System.IO;

namespace Ark.ViewModels
{
    public class ViewModelSearchPage : DependencyObject
    {
        public ioInput CurrentInput
        {
            get { return (ioInput)GetValue(CurrentInputProperty); }
            set { SetValue(CurrentInputProperty, value); }
        }
        public static readonly DependencyProperty CurrentInputProperty =
            DependencyProperty.Register("CurrentInput", typeof(ioInput), typeof(ViewModelSearchPage), new PropertyMetadata(null));



        #region Dependency properties

        public bool UserDraggedFSOsOnDesktopIcon
        {
            get { return (bool)GetValue(UserDraggedFSOsOnDesktopIconProperty); }
            set { SetValue(UserDraggedFSOsOnDesktopIconProperty, value); }
        }
        public static readonly DependencyProperty UserDraggedFSOsOnDesktopIconProperty = DependencyProperty.Register("UserDraggedFSOsOnDesktopIcon", typeof(bool), typeof(ViewModelSearchPage), new PropertyMetadata(false));

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



        public bool UserIsArchiving
        {
            get { return (bool)GetValue(InputInProgressProperty); }
            set { SetValue(InputInProgressProperty, value); }
        }
        public static readonly DependencyProperty InputInProgressProperty = DependencyProperty.Register("InputInProgress", typeof(bool), typeof(ViewModelSearchPage), new UIPropertyMetadata(false));

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
        
        #endregion

        public void ClearInput()
        {
            CurrentInput = null;
            UserIsArchiving = false;
        }

        public void SetInput(ioInput input)
        {
            CurrentInput = input;
            UserIsArchiving = true;
        }


        public ViewModelSearchPage(ViewModelHelp vmHelp, ioInput input = null)
        {
            App.CurrentVMHelp = vmHelp;
            if (input != null)
            {
                SetInput(input);
            }



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

        public void OpenArchiveItem(ArchiveItem item)
        {
            if (UserIsArchiving)
            {
                ioFSOMover mover = new ioFSOMover();
                mover.MoveFSOsToArchiveItem(CurrentInput.FSOPaths, item);
                ClearInput();
            }
            else { 
                System.Diagnostics.Process.Start(item.DirInfo.FullName);
            }
        }

    }
}
