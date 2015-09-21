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
            if (System.IO.Directory.Exists(Properties.Settings.Default.WordRootFolder) == false) {
                MessageBox.Show("Root folder not found."); //# Redirect to settings
            }
            else
            {
                mainFrame.Navigate(new pageSearch());
            }
            
            
        }

    }
}
