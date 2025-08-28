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

namespace DjmaxRandomSelectorV.Ui.Controls
{
    public class SettingControl : Control
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(string),
            typeof(SettingControl),
            new FrameworkPropertyMetadata("LABEL"));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        static SettingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingControl), new FrameworkPropertyMetadata(typeof(SettingControl)));
        }
    }
}
