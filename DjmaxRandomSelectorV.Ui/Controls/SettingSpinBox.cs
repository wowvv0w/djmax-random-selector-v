using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class SettingSpinBox : SettingControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(int),
            typeof(SettingSpinBox),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MininumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(int),
            typeof(SettingSpinBox),
            new FrameworkPropertyMetadata(0));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(int),
            typeof(SettingSpinBox),
            new FrameworkPropertyMetadata(10));
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(
            "ValueText",
            typeof(string),
            typeof(SettingSpinBox),
            new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsCircularProperty = DependencyProperty.Register(
            "IsCircular",
            typeof(bool),
            typeof(SettingSpinBox),
            new PropertyMetadata(false));
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay",
            typeof(int),
            typeof(SettingSpinBox),
            new PropertyMetadata(250));
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            "Interval",
            typeof(int),
            typeof(SettingSpinBox),
            new PropertyMetadata(33));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public int Minimum
        {
            get => (int)GetValue(MininumProperty);
            set => SetValue(MininumProperty, value);
        }
        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }
        public bool IsCircular
        {
            get => (bool)GetValue(IsCircularProperty);
            set => SetValue(IsCircularProperty, value);
        }
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        private RepeatButton? _prevValueButtonElement;
        public RepeatButton? PrevValueButtonElement
        {
            get => _prevValueButtonElement;
            set
            {
                if (_prevValueButtonElement is not null)
                {
                    _prevValueButtonElement.Click -= new RoutedEventHandler(PrevValueButtonElement_Click);
                }
                _prevValueButtonElement = value;
                if (_prevValueButtonElement is not null)
                {
                    _prevValueButtonElement.Click += new RoutedEventHandler(PrevValueButtonElement_Click);
                }
            }
        }

        private RepeatButton? _nextValueButtonElement;
        public RepeatButton? NextValueButtonElement
        {
            get => _nextValueButtonElement;
            set
            {
                if (_nextValueButtonElement is not null)
                {
                    _nextValueButtonElement.Click -= new RoutedEventHandler(NextValueButtonElement_Click);
                }
                _nextValueButtonElement = value;
                if (_nextValueButtonElement is not null)
                {
                    _nextValueButtonElement.Click += new RoutedEventHandler(NextValueButtonElement_Click);
                }
            }
        }

        static SettingSpinBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingSpinBox), new FrameworkPropertyMetadata(typeof(SettingSpinBox)));
        }

        public override void OnApplyTemplate()
        {
            PrevValueButtonElement = GetTemplateChild("PrevValueButton") as RepeatButton;
            NextValueButtonElement = GetTemplateChild("NextValueButton") as RepeatButton;
        }

        private void SetValue(int newValue)
        {
            if (Minimum <= newValue && newValue <= Maximum)
            {
                Value = newValue;
            }
            else if (IsCircular)
            {
                int count = Maximum - Minimum + 1;
                Value = (newValue + count) % count;
            }
        }

        private void PrevValueButtonElement_Click(object sender, RoutedEventArgs e)
        {
            SetValue(Value - 1);
        }
        private void NextValueButtonElement_Click(object sender, RoutedEventArgs e)
        {
            SetValue(Value + 1);
        }
    }
}
