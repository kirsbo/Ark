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
using Ark.ViewModels;
using Ark.Models;

namespace Ark.UserControls
{
    /// <summary>
    /// Interaction logic for UCHowToPanel.xaml
    /// </summary>
    public partial class UCHowToPanel : UserControl
    {
        ViewModelSearchPage _parentVM;
        public UCHowToPanel(ViewModelSearchPage parentVM)
        {
            InitializeComponent();
            _parentVM = parentVM;
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            _parentVM.CreateAndAddNewWord(new WordType(WordType.WordTypeEnum.Folder));
        }

        private void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            _parentVM.CreateAndAddNewWord(new WordType(WordType.WordTypeEnum.Note));
        }
    }
}
