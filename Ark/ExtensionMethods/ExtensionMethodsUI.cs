using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Ark.Models;

namespace Ark
{
    public static class ExtensionMethodsUI
    {

        public static void SelectPreviousItem(this ListBox listBox)
        {
           if (listBox.SelectedIndex == 0) { return; } //Making sure we're not already at the top of the listbox.

            listBox.SelectedIndex = listBox.SelectedIndex - 1;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
    
        public static void SelectNextItem(this ListBox listBox)
        {
            if (listBox.SelectedIndex == listBox.Items.Count - 1) { return; } //Making sure we're not already at the bottom of the listbox.

            listBox.SelectedIndex = listBox.SelectedIndex + 1;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }

    }
}
