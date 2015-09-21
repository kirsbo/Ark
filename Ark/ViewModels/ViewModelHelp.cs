using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Ark.ViewModels
{
    public class ViewModelHelp : DependencyObject
    {
        public enum HelpbarTypeEnum
        {
            Neutral,
            Negative,
            Positive
        }

        public bool HelpbarIsAnimating
        {
            get { return (bool)GetValue(HelpbarIsAnimatingProperty); }
            set { SetValue(HelpbarIsAnimatingProperty, value); }
        }
        public static readonly DependencyProperty HelpbarIsAnimatingProperty = DependencyProperty.Register("HelpbarIsAnimating", typeof(bool), typeof(ViewModelHelp));

        public HelpbarTypeEnum HelpbarType
        {
            get { return (HelpbarTypeEnum)GetValue(HelpbarTypeProperty); }
            set { SetValue(HelpbarTypeProperty, value); }
        }
        public static readonly DependencyProperty HelpbarTypeProperty = DependencyProperty.Register("HelpbarType", typeof(HelpbarTypeEnum), typeof(ViewModelHelp), new PropertyMetadata(default(HelpbarTypeEnum)));

        public string HelpbarText
        {
            get { return (string)GetValue(HelpbarTextProperty); }
            set { SetValue(HelpbarTextProperty, value); }
        }
        public static readonly DependencyProperty HelpbarTextProperty = DependencyProperty.Register("HelpbarText", typeof(string), typeof(ViewModelHelp), new PropertyMetadata(""));

        public void ShowHelpbar(string text, HelpbarTypeEnum helpbarType)
        {
            HelpbarText = text;
            HelpbarType = helpbarType;
            HelpbarIsAnimating = true; //A quick switch makes the animation start in the view.
            HelpbarIsAnimating = false;
        }

        public void ShowNeutralHelpbar(string text)
        {
            ShowHelpbar(text, HelpbarTypeEnum.Neutral);
        }

        public void ShowPositiveHelpbar(string text)
        {
            ShowHelpbar(text, HelpbarTypeEnum.Positive);
        }

        public void ShowNegativeHelpbar(string text)
        {
            ShowHelpbar(text, HelpbarTypeEnum.Negative);
        }
    }
}
