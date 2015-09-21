using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Collections.ObjectModel;
using Ark.ViewModels;
using Ark.Models;
using Ark.IO;
using System.Windows.Media.Animation;

namespace Ark
{
    /// <summary>
    /// Interaction logic for pageSearch.xaml
    /// </summary>
    public partial class pageSearch : Page
    {
        private ViewModelSearchPage vmSearchPage;
        private Word currentSelectedWord
        {
            get
            {
                return (Word)listResult.SelectedItem;
            }
        }

        private void loadArgs()
        {
             
        }

        public pageSearch()
        {
            InitializeComponent();
            vmSearchPage = new ViewModelSearchPage(vmHelp);
            this.DataContext = vmSearchPage;

            //Handling command arguments reading, and setting of focus to textsearch if there's no commandline arguments.
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => {
                string[] args = Environment.GetCommandLineArgs();

                if (args.Length > 1)
                {
                    InputCommandLineArgs inputArgs = new InputCommandLineArgs(args);
                    manageIOinput(inputArgs);
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate()
                    {
                        txtSearch.Focus();
                        Keyboard.Focus(txtSearch);
                    }));
                }
            } ));

            
        }

        private void listResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            openWord(currentSelectedWord);
        }

        private void controls_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (vmSearchPage.UserIsRenamingWord) { FinishRename(); break; }
                    openWord((Word)listResult.SelectedItem);
                    break;
                case Key.F2:
                    if (vmSearchPage.UserIsRenamingWord) { AbortRename(); break; }
                    if (!vmSearchPage.UserIsRenamingWord) { StartRename(); break; }
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
                        if (vmSearchPage.UserIsRenamingWord == true) { break; }
                        if (currentSelectedWord == null) { break; }
                        e.Handled = true;
                        copyWordToClipboard(currentSelectedWord);
                        
                    }
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //If user is renaming, pasting will insert plain text into the textbox. If user is not renaming, we attempt to paste to the selected word or a new word.
                        if (vmSearchPage.UserIsRenamingWord == true) { break; }
                        
                        e.Handled = true;
                        OnPaste();
                    }
                    break;
            }
        }

        #region Copy/Pasting
        private void copyWordToClipboard(Word word)
        {
            StringCollection selectedWordPath = new StringCollection();
            if (word.Type.IsFolder) {
                selectedWordPath.Add(word.DirInfo.FullName);
            }
            if (word.Type.IsFile) {
                selectedWordPath.Add(word.FileInfo.FullName);
            }

            Clipboard.SetFileDropList(selectedWordPath);
            vmHelp.ShowPositiveHelpbar(String.Format("I have copied {0} to the clipboard.", word.Name));
        }

        private void OnPaste()
        {
            InputClipboard inputClip = new InputClipboard();

            if (currentSelectedWord == null)
            {
                manageIOinput(inputClip);
            }
            else
            {
                managePasteInput(inputClip);
            }
        }

        private void managePasteInput(InputClipboard inputClip)
        {
            if (inputClip.IsFSO)
            {
                ioFSOMover fileSaver = new ioFSOMover();
                fileSaver.MoveFSOsToWord(inputClip.FSOPaths, currentSelectedWord);
            }

            if (inputClip.IsText)
            {
                ioNoteTextSaver noteTextAdder = new ioNoteTextSaver();
                noteTextAdder.PasteTextToWord(inputClip.ClipboardText, currentSelectedWord);
            }
        }

        #endregion

        public void manageIOinput(ioInput input)
        {
            if (input is InputCommandLineArgs)
            {
                if (input.IsSingleNoteFile == false)
                {
                    vmSearchPage.UserDraggedFSOsOnDesktopIcon = true;
                    return;
                }
                //Fall through, if user drag-dropped a single text file, as then we don't need to open "add to existing or create new" page.
            }

            if (input.IsSingleNoteFile)
            {
                vmSearchPage.CreateAndAddNoteFromFile(input.SingleFileDI);
                return;
            }

            if (input.IsSingleFile)
            {
                vmSearchPage.CreateAndAddFolderFromList(input.FSOPaths);
                return;
            }

            if (input.IsFSO == false)
            {
                InputClipboard inputClip = (InputClipboard)input;
                vmSearchPage.CreateAndAddNoteFromString(inputClip.ClipboardText);
                return;
            }

            if (input.IsSingleFolder)
            {
                vmSearchPage.CreateAndAddFolderFromFolder(input.SingleFolderDI);
                return;
            }

            if (input.IsMultipleFSOs)
            {
                vmSearchPage.CreateAndAddFolderFromList(input.FSOPaths);
                return;
            }
        }

        private void openWord(Word word)
        {
            vmSearchPage.OpenWord(word);
        }

        #region DragDrop handling

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            vmSearchPage.UserIsDragging = true;
            vmSearchPage.UserIsDraggingNote = inputDD.IsSingleNoteFile;
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
            Word word = (Word)(sender as Grid).DataContext;
            listResult.SelectedItem = word;
        }

        private void canvasDragDropPanel_Drop(object sender, DragEventArgs e)
        {
            vmSearchPage.UserIsDragging = false;
            vmSearchPage.UserIsDraggingNote = false;
            vmSearchPage.UserIsDraggingOnDragDropPanel = false;

            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            manageIOinput(inputDD);
        }

        private void resultItem_Drop(object sender, DragEventArgs e)
        {
            Word word = (Word)(sender as Grid).DataContext;
            InputDragDrop inputDD = new InputDragDrop(e);
            if (inputDD.ObjectHasDataPresent == false) { return; }

            ioFSOMover fileSaver = new ioFSOMover();
            fileSaver.MoveFSOsToWord(inputDD.FSOPaths, word);
        }
        #endregion

        #region Renaming handling
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            StartRename();
        }

        private void StartRename()
        {
            vmSearchPage.UserIsRenamingWord = true;
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
            Word selectedWord = currentSelectedWord;

            ListBoxItem wordListItem = (ListBoxItem)listResult.ItemContainerGenerator.ContainerFromItem(listResult.SelectedItem);
            TextBox wordNameTextBox = WpfUtils.FindVisualChild<TextBox>(wordListItem);
            TextBlock wordNameLabel = WpfUtils.FindVisualChild<TextBlock>(wordListItem);

            IO.ioRenamer renamer = new IO.ioRenamer();

            if (renamer.AttemptRename(currentSelectedWord, wordNameTextBox.Text))
            {
                //This just syncs label with the new name (without the below line, the new name wouldn't get reflected in the label until user entered a new search query, thereby updating the collection).
                wordNameLabel.Text = wordNameTextBox.Text;
            }

            vmSearchPage.UserIsRenamingWord = false;
        }

        private void AbortRename()
        {
            vmSearchPage.UserIsRenamingWord = false;
        }

        private void listResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Selecting is changing.");
            if (vmSearchPage.UserIsRenamingWord) { AbortRename(); }
        }

        #endregion

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            vmSearchPage.CreateAndAddNewWord(new WordType(WordType.WordTypeEnum.Folder));
        }

        private void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            vmSearchPage.CreateAndAddNewWord(new WordType(WordType.WordTypeEnum.Note));
        }

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
            if (txtSearch.Text.Length > 0)
            {
                vmSearchPage.UserIsSearching = true;
                vmSearchPage.UserIsNotSearching = false;
            }
            else
            {
                vmSearchPage.UserIsSearching = false;
                vmSearchPage.UserIsNotSearching = true;
                vmSearchPage.FilteredWords = new List<Word>(); //Emptying results list
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vmSearchPage.UserIsSearching = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            belowPanel.Content = new Ark.UserControls.UCHowToPanel(vmSearchPage);

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            canvasDragDropPanel_Drop(sender, e);
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Word callingWord = (Word)sender;
            copyWordToClipboard(callingWord);
        }

        private void btnPasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            OnPaste();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "Test";
        }


    }
}
