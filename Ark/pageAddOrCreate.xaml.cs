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
using Ark.IO;

namespace Ark
{
    /// <summary>
    /// Interaction logic for pageAddOrCreate.xaml
    /// </summary>
    public partial class pageAddOrCreate : Page
    {
        private InputCommandLineArgs InputCmd;
        public pageAddOrCreate(InputCommandLineArgs inputCmd)
        {
            InitializeComponent();
            InputCmd = inputCmd;
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            InputCmd.CommandLineDropDestination = InputCommandLineArgs.CommandlineDropDestinationEnum.NewFolder;
        }

        private void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            InputCmd.CommandLineDropDestination = InputCommandLineArgs.CommandlineDropDestinationEnum.ExistingFolder;
        }
    }
}
