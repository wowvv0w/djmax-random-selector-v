using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using DjmaxRandomSelectorV.ViewModels;

namespace DjmaxRandomSelectorV.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainViewModel ViewModel { get; }

        public MainView()
        {
            InitializeComponent();
            DataContext = this;
            ViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
        }
    }
}
