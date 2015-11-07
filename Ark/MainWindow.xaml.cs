using System;
using System.Collections.Generic;
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
using System.IO;
using Ark.IO;

namespace Ark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.Directory.Exists(Properties.Settings.Default.ArchiveRootFolder) == false) {
                mainFrame.Navigate(new pageSettings());
                return;
            }

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                InputCommandLineArgs inputArgs = new InputCommandLineArgs(args);
                mainFrame.Navigate(new pageSearch(inputArgs)); //Not using App.Globalnavigator as it hasn't been initialized at this point.
            }
            else
            {
                mainFrame.Navigate(new pageSearch()); //Not using App.Globalnavigator as it hasn't been initialized at this point.
            }
            
            
            
        }

    }
}
