using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
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

        private void loadArgs()
        {

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
            if (srcType == typeof(ListViewItem) || srcType == typeof(GridViewRowPresenter) || srcType==typeof(Grid))
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
                    selectArchiveItem((ArchiveItem)listResult.SelectedItem);
                    break;
                case Key.F2:
                    if (vmArchiveItems.UserIsRenamingFolder) { AbortRename(); break; }
                    if (!vmArchiveItems.UserIsRenamingFolder) { StartRename(); break; }
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
                        OnPaste(false);
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

        private void OnPaste(bool pasteOnSpecificItem)
        {
            InputClipboard inputClip = new InputClipboard();

            managePasteInput(inputClip, pasteOnSpecificItem);
        }

        private void managePasteInput(InputClipboard inputClip, bool pasteOnSpecificItem)
        {
            if (!inputClip.IsFSO) { return; }
            if (pasteOnSpecificItem)
            {
                ioFSOMover fileSaver = new ioFSOMover();
                fileSaver.MoveFSOsToArchiveItem(inputClip.FSOPaths, currentSelectedItem);
            }
            else
            {
                if (vmSearchPage.CurrentInput == null) {
                    vmSearchPage.SetInput(inputClip);
                }
                else
                {
                    vmSearchPage.CurrentInput.AddPaths(inputClip.FSOPaths);
                }
            }
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

        private void canvasDragDropPanel_DragEnter(object sender, DragEventArgs e)
        {
            vmSearchPage.UserIsDraggingOnDragDropPanel = true;
        }

        private void canvasDragDropPanel_DragLeave(object sender, DragEventArgs e)
        {
            vmSearchPage.UserIsDraggingOnDragDropPanel = false;
        }

        private void resultItem_DragEnter(object sender, DragEventArgs e)
        {
            ArchiveItem word = (ArchiveItem)(sender as Grid).DataContext;
            listResult.SelectedItem = word;
        }

        private void canvasDragDropPanel_Drop(object sender, DragEventArgs e)
        {
            //## Needs rewriting 
            vmSearchPage.UserIsDragging = false;
            vmSearchPage.UserIsDraggingOnDragDropPanel = false;

            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            //App.GlobalNavigator.Navigate(new pageArchive(inputDD));
        }

        private void resultItem_Drop(object sender, DragEventArgs e)
        {
            ArchiveItem word = (ArchiveItem)(sender as Grid).DataContext;
            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            ioFSOMover fileSaver = new ioFSOMover();
            fileSaver.MoveFSOsToArchiveItem(inputDD.FSOPaths, word);
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
            txtSearch.Text = "";
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Code for hiding/showing help text and settings/help icons
            if (txtSearch.Text.Length > 0)
            {
                vmSearchPage.UserIsSearching = true;
                vmSearchPage.UserIsNotSearching = false;
            }
            else
            {
                vmSearchPage.UserIsSearching = false;
                vmSearchPage.UserIsNotSearching = true;
            }
        }



        private void Grid_Drop(object sender, DragEventArgs e)
        {
            canvasDragDropPanel_Drop(sender, e);
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            ArchiveItem selectedItem = (ArchiveItem)sender;
            copyArchiveItemToClipboard(selectedItem);
        }

        private void btnPasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            OnPaste(true);
        }


        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            vmArchiveItems.CreateNewFolder();
        }

        private void btnCancelArchiving_Click(object sender, RoutedEventArgs e)
        {
            vmSearchPage.ClearInput(); 
        }


        #region MenuItem event handlers
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            vmArchiveItems.CreateNewFolder();
        }

        #endregion

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test click");
        }

        private void btnNewFolder_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //List<string> fsos = vmSearchPage.CurrentInput.FSOPaths;
            vmSearchPage.CurrentInput.RemovePath(listInput.SelectedItem.ToString());

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            vmSearchPage.CurrentInput.RemovePath(listInput.SelectedItem.ToString());
            if (vmSearchPage.CurrentInput.FSOPaths.Count == 0) { vmSearchPage.ClearInput(); }
        }
    }
}
