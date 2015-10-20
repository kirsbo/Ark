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

namespace Ark.CustomControls
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        private Button _controlButton;

        public ImageButton()
        {
            InitializeComponent();
            this.DataContext = this;

            _controlButton = GetTemplateChild("buttonControl") as Button;

            if (buttonControl != null)
            { buttonControl.Click += onClick; }

        }

        #region Dependency properties

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new PropertyMetadata(null));
        #endregion

        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClickCustom", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ImageButton));

        public event RoutedEventHandler OnClickCustom
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnClickEvent));
        }

     }
}
