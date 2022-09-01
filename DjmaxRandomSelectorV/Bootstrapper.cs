using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using System.Windows;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
