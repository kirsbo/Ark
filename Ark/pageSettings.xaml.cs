using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ark
{
    /// <summary>
    /// Interaction logic for pageSettings.xaml
    /// </summary>
    public partial class pageSettings : Page
    {
        public pageSettings()
        {
            InitializeComponent();

            txtArchiveFolderPath.Text = Properties.Settings.Default.ArchiveRootFolder;
            checkEnableSounds.IsChecked = Properties.Settings.Default.PlaySounds;
            checkCloseAfterArchiving.IsChecked = Properties.Settings.Default.CloseAfterArchiving;
            checkCloseAfterOpening.IsChecked = Properties.Settings.Default.CloseAfterOpening;
        }

        private void btnBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog();

            WinForms.DialogResult dialogResult = folderDialog.ShowDialog();
            if (dialogResult == WinForms.DialogResult.Cancel) { return; }

            txtArchiveFolderPath.Text = folderDialog.SelectedPath;

        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);
            App.GlobalNavigator.Navigate(new pageSearch());
        }

        private void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SoundEffects.Play(SoundEffects.EffectEnum.Click);

            Properties.Settings.Default.ArchiveRootFolder = txtArchiveFolderPath.Text;
            Properties.Settings.Default.PlaySounds = (bool)checkEnableSounds.IsChecked;
            Properties.Settings.Default.CloseAfterArchiving = (bool)checkCloseAfterArchiving.IsChecked;
            Properties.Settings.Default.CloseAfterOpening = (bool)checkCloseAfterOpening.IsChecked;
            Properties.Settings.Default.Save();

            App.GlobalNavigator.Navigate(new pageSearch());
        }

    }
}
