using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DjmaxRandomSelectorV.Ui.Controls
{
    public class SettingToggleButton : ToggleButton
    {
        static SettingToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingToggleButton), new FrameworkPropertyMetadata(typeof(SettingToggleButton)));
        }
    }
}
