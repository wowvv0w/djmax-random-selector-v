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
    public class SettingSlider : SettingControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MininumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(0.0));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(10.0));
        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
            "TickFrequency",
            typeof(double),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(1.0));
        public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register(
            "IsSnapToTickEnabled",
            typeof(bool),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(
            "ValueText",
            typeof(string),
            typeof(SettingSlider),
            new FrameworkPropertyMetadata(null));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public double Minimum
        {
            get => (double)GetValue(MininumProperty);
            set => SetValue(MininumProperty, value);
        }
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public double TickFrequency
        {
            get => (double)GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }
        public bool IsSnapToTickEnabled
        {
            get => (bool)GetValue(IsSnapToTickEnabledProperty);
            set => SetValue(IsSnapToTickEnabledProperty, value);
        }
        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }

        static SettingSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingSlider), new FrameworkPropertyMetadata(typeof(SettingSlider)));
        }
    }
}
