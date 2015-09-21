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

namespace Ark.UserControls
{
    /// <summary>
    /// Interaction logic for ArkButton.xaml
    /// </summary>
    public partial class ArkButton : UserControl
    {
        public enum ButtonTypeEnum
        {
            Grey,
            Green,
            Red,
            NextButton,
            CancelButton
        }

        public string Text { get; set; }
        //public ButtonTypeEnum ButtonType { get; set; }


        public ButtonTypeEnum ButtonType
        {
            get { return (ButtonTypeEnum)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ButtonTypeEnum), typeof(ArkButton));



        public ArkButton()
        {
            InitializeComponent();
            button.Content = Text;
            Style style;
            switch (ButtonType)
            {
                case ButtonTypeEnum.Grey:
                    style = (Style)this.FindResource("StyleButton_Grey");
                    break;
                case ButtonTypeEnum.Green:
                    style = (Style)this.FindResource("StyleButton_Green");
                    break;
                case ButtonTypeEnum.Red:
                    style = (Style)this.FindResource("StyleButton_Red");
                    break;
                case ButtonTypeEnum.NextButton:
                    style = (Style)this.FindResource("StyleButton_NextButton");
                    break;
                case ButtonTypeEnum.CancelButton:
                    style = (Style)this.FindResource("StyleButton_CancelButton");
                    break;
                default:
                    style = (Style)this.FindResource("StyleButton_Grey");
                    break;
            }
            button.Style = style;
        }
    }
}
