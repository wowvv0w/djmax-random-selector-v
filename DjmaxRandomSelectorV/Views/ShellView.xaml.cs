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
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.DependencyInjection;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.ViewModels;

namespace DjmaxRandomSelectorV.Views
{
    /// <summary>
    /// ShellView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellViewModel ViewModel { get; }

        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
            ViewModel = Ioc.Default.GetRequiredService<ShellViewModel>();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var config = Ioc.Default.GetRequiredService<Dmrsv3Configuration>();
            config.Position = new double[2] { Top, Left };
            Close();
        }
    }
}
