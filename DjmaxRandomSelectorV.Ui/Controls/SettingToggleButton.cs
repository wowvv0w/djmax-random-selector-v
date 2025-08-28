using System.Text;
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
    public class SettingToggleButton : SettingControl
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked",
            typeof(bool),
            typeof(SettingToggleButton),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TextWhenCheckedProperty = DependencyProperty.Register(
            "TextWhenChecked",
            typeof(string),
            typeof(SettingToggleButton),
            new FrameworkPropertyMetadata("ON"));
        public static readonly DependencyProperty TextWhenNotCheckedProperty = DependencyProperty.Register(
            "TextWhenNotChecked",
            typeof(string),
            typeof(SettingToggleButton),
            new FrameworkPropertyMetadata("OFF"));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public string TextWhenChecked
        {
            get => (string)GetValue(TextWhenCheckedProperty);
            set => SetValue(TextWhenCheckedProperty, value);
        }
        public string TextWhenNotChecked
        {
            get => (string)GetValue(TextWhenNotCheckedProperty);
            set => SetValue(TextWhenNotCheckedProperty, value);
        }

        static SettingToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingToggleButton), new FrameworkPropertyMetadata(typeof(SettingToggleButton)));
        }
    }
}
