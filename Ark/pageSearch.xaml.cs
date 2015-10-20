using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ark.ViewModels;
using Ark.Models;
using Ark.IO;


namespace Ark
{
    /// <summary>
    /// Interaction logic for pageSearch.xaml
    /// </summary>
    public partial class pageSearch : Page
    {
        private ViewModelSearchPage vmSearchPage;
        private ViewModelArchiveItems vmArchiveItems;
        private ArchiveItem currentSelectedItem
        {
            get
            {
                return (ArchiveItem)listResult.SelectedItem;
            }
        }

        public pageSearch(ioInput input = null)
        {
            InitializeComponent();
            vmSearchPage = new ViewModelSearchPage(vmHelp, input);

            this.DataContext = vmSearchPage;

            vmArchiveItems = new ViewModelArchiveItems();
            listResult.DataContext = vmArchiveItems;
            txtSearch.DataContext = vmArchiveItems;

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate ()
            {
                txtSearch.Focus();
                Keyboard.Focus(txtSearch);
            }));
        }

        private void listResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //All this code is to prevent double click event triggering when user double clicks the custom scrollbar in the results list.
            var src = VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource);
            var srcType = src.GetType();
            if (srcType == typeof(ListViewItem) || srcType == typeof(GridViewRowPresenter) || srcType == typeof(Grid))
            {
                selectArchiveItem(currentSelectedItem);
            }

        }

        private void controls_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (vmArchiveItems.UserIsRenamingFolder) { FinishRename(); break; }
                    if (listResult.SelectedItem != null) { selectArchiveItem((ArchiveItem)listResult.SelectedItem); }
                    break;
                case Key.Escape:
                    if (vmArchiveItems.UserIsRenamingFolder) { AbortRename(); }
                    break;
                case Key.F2:
                    if (vmArchiveItems.UserIsRenamingFolder) { AbortRename(); break; }
                    if ((!vmArchiveItems.UserIsRenamingFolder) && (listResult.SelectedItem != null)) { StartRename(); break; }
                    break;
                case Key.Down:
                    listResult.SelectNextItem();
                    break;
                case Key.Up:
                    listResult.SelectPreviousItem();
                    break;
                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (vmArchiveItems.UserIsRenamingFolder == true) { break; }
                        if (currentSelectedItem == null) { break; }
                        e.Handled = true;
                        copyArchiveItemToClipboard(currentSelectedItem);
                    }
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //If user is renaming, pasting will insert plain text into the textbox. If user is not renaming, we attempt to paste to the selected word or a new word.
                        if (vmArchiveItems.UserIsRenamingFolder == true) { break; }

                        e.Handled = true;
                        pasteInput();
                    }
                    break;
            }
        }

        #region Copy/Pasting
        private void copyArchiveItemToClipboard(ArchiveItem archiveItem)
        {
            StringCollection selectedItemPath = new StringCollection();
            selectedItemPath.Add(archiveItem.DirInfo.FullName);

            Clipboard.SetFileDropList(selectedItemPath);
            vmHelp.ShowPositiveHelpbar(String.Format("I have copied {0} to the clipboard.", archiveItem.Name));
        }

        private void addInput(ioInput input)
        {
            if (vmSearchPage.CurrentInput == null)
            {
                vmSearchPage.SetInput(input);
            }
            else
            {
                vmSearchPage.CurrentInput.AddPaths(input.FSOPaths);
            }
        }

        private void pasteInput()
        {
            InputClipboard inputFromClipboard = new InputClipboard();
            if (!inputFromClipboard.IsFSO) { return; }

            addInput(inputFromClipboard);
        }

        #endregion

        private void selectArchiveItem(ArchiveItem archiveItem)
        {
            vmSearchPage.OpenArchiveItem(archiveItem);
        }

        #region DragDrop handling

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            vmSearchPage.UserIsDragging = true;
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            vmSearchPage.UserIsDragging = false;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            vmSearchPage.UserIsDragging = false;

            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            addInput(inputDD);
        }

        private void resultItem_DragEnter(object sender, DragEventArgs e)
        {
            ArchiveItem archiveItem = (ArchiveItem)(sender as Grid).DataContext;
            listResult.SelectedItem = archiveItem;
        }



        private void resultItem_Drop(object sender, DragEventArgs e)
        {
            ArchiveItem archiveItem = (ArchiveItem)(sender as Grid).DataContext;
            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            ioFSOMover fileSaver = new ioFSOMover();

            e.Handled = true;
            fileSaver.MoveFSOsToArchiveItem(inputDD.FSOPaths, archiveItem);
            if (Properties.Settings.Default.CloseAfterArchiving) { Application.Current.Shutdown(); }
        }
        #endregion

        #region Renaming handling
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            StartRename();
        }

        private void StartRename()
        {
            vmArchiveItems.UserIsRenamingFolder = true;
            //Below is to make sure that the textbox holds the same value as the label (word name) when starting to rename. 
            //These could get out of sync if user first renames one textbox, but does not press enter (i.e. saving the rename), and then later tries to rename the same item again:
            //In this case the previously edited value will be shown, as there's only one-way databinding on the textbox (to avoid updating the word object before user has accepted the rename).
            ListBoxItem wordListItem = (ListBoxItem)listResult.ItemContainerGenerator.ContainerFromItem(listResult.SelectedItem);
            TextBox wordNameTextBox = WpfUtils.FindVisualChild<TextBox>(wordListItem);
            TextBlock wordNameLabel = WpfUtils.FindVisualChild<TextBlock>(wordListItem);
            wordNameTextBox.Text = wordNameLabel.Text;
        }

        private void FinishRename()
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Thump);
            ArchiveItem selectedWord = currentSelectedItem;

            ListBoxItem archiveListItem = (ListBoxItem)listResult.ItemContainerGenerator.ContainerFromItem(listResult.SelectedItem);
            TextBox archiveItemNameTextBox = WpfUtils.FindVisualChild<TextBox>(archiveListItem);
            TextBlock archiveItemNameLabel = WpfUtils.FindVisualChild<TextBlock>(archiveListItem);

            IO.ioRenamer renamer = new IO.ioRenamer();

            string newName = archiveItemNameTextBox.Text;

            if (renamer.AttemptRename(currentSelectedItem, newName))
            {
                //This just syncs label with the new name (without the below line, the new name wouldn't get reflected in the label until user entered a new search query, thereby updating the collection).
                archiveItemNameLabel.Text = newName;
                vmArchiveItems.SearchFilter = newName;
            }

            vmArchiveItems.UserIsRenamingFolder = false;
        }

        private void AbortRename()
        {
            vmArchiveItems.UserIsRenamingFolder = false;
        }

        private void listResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vmArchiveItems.UserIsRenamingFolder) { AbortRename(); }
        }

        #endregion


        #region Various UI
        
        private void txtItemName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox thisTextBox = (TextBox)sender;
            if (thisTextBox.Text.StartsWith("Untitled "))
            {
                thisTextBox.SelectAll();
            }
            else
            {
                thisTextBox.CaretIndex = thisTextBox.Text.Length;
            }
        }

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            txtSearch.Text = "";
            txtSearch.Focus();
        }

        #endregion  

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            ArchiveItem selectedItem = (ArchiveItem)sender;
            copyArchiveItemToClipboard(selectedItem);
        }

        private void btnPasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            pasteInput();
        }



        

        #region MenuItem event handlers
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            Application.Current.Shutdown();
        }

        private void MenuItem_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            vmArchiveItems.CreateNewFolder();
        }


        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            App.GlobalNavigator.Navigate(new pageSettings());
        }


        private void MenuItem_ArchiveFile_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = true;

            Nullable<bool> dialogResult = fileDialog.ShowDialog();
            if (dialogResult != true) { return; }

            List<string> selectedFiles = new List<string>(fileDialog.FileNames);
            vmSearchPage.CurrentInput.AddPaths(selectedFiles);
        }

        private void MenuItem_ArchiveFolder_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog();

            WinForms.DialogResult dialogResult = folderDialog.ShowDialog();
            if (dialogResult == WinForms.DialogResult.Cancel) { return; }

            vmSearchPage.CurrentInput.AddPaths(new List<string> { folderDialog.SelectedPath });


        }






        #endregion

        #region "Input list buttons"

        private void btnArchiveWithSaveName_OnClick(object sender, RoutedEventArgs e)
        {
            string newPath = System.IO.Path.Combine(Properties.Settings.Default.ArchiveRootFolder, vmSearchPage.CurrentInput.SingleFolderDI.Name);
            if (System.IO.Directory.Exists(newPath)) { vmHelp.ShowNegativeHelpbar("I cannot archive this folder as there is already a folder with the same name."); return; }

            vmArchiveItems.ArchiveWithSameName(vmSearchPage.CurrentInput.SingleFolderDI);

            vmSearchPage.ClearInput();
        }

        private void btnNewFolder_OnClick(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            vmArchiveItems.CreateNewFolder();
        }

        private void btnCancelArchiving_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            vmSearchPage.ClearInput();
        }

        

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            vmSearchPage.CurrentInput.RemovePath(listInput.SelectedItem.ToString());
            if (vmSearchPage.CurrentInput.FSOPaths.Count == 0) { vmSearchPage.ClearInput(); }
        }

        #endregion  
        private void button_Click(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Archive);

        }

       

    }
}
