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
            set {
                SetValue(CurrentInputProperty, value);
                
                if (value == null) {
                    UserIsArchiving = false;
                }
                else {
                    UserIsArchiving = true;
                }
                
            }
        }
        public static readonly DependencyProperty CurrentInputProperty =
            DependencyProperty.Register("CurrentInput", typeof(ioInput), typeof(ViewModelSearchPage), new PropertyMetadata(null));

        ViewModelArchiveItems vmArchive; 

        #region Dependency properties

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

        #endregion

        public ViewModelSearchPage(ViewModelHelp vmHelp, ViewModelArchiveItems vmArchiveItems, ioInput input = null)
        {
            App.CurrentVMHelp = vmHelp;
            vmArchive = vmArchiveItems;

            if (input != null)
            {
                CurrentInput = input; 
            }
        }

        public void SelectArchiveItem(ArchiveItem selectedItem)
        {
            if (UserIsArchiving)
            {
                vmArchive.ArchiveInput(CurrentInput, selectedItem.DirInfo);
                CurrentInput = null;
            }
            else
            {
                SoundEffects.Play(SoundEffects.EffectEnum.Click);
                System.Diagnostics.Process.Start(selectedItem.DirInfo.FullName);

                if (Properties.Settings.Default.CloseAfterOpening) { Application.Current.Shutdown(); }
            }
        }

        public void AddInput(ioInput input) 
        {
            if (CurrentInput == null)
            {
                CurrentInput = input;
            }
            else
            {
                CurrentInput.AddPaths(input.FSOPaths);
            }
        }

    }
}
