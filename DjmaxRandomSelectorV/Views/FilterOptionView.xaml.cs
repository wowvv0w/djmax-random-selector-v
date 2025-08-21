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
using CommunityToolkit.Mvvm.DependencyInjection;
using DjmaxRandomSelectorV.ViewModels;

namespace DjmaxRandomSelectorV.Views
{
    /// <summary>
    /// FilterOptionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FilterOptionView : UserControl
    {
        public FilterOptionViewModel ViewModel { get; }

        public FilterOptionView()
        {
            InitializeComponent();
            DataContext = this;
            ViewModel = Ioc.Default.GetRequiredService<FilterOptionViewModel>();
        }
    }
}
