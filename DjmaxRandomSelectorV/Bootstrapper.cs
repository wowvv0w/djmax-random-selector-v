using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows;
using System.Xml.Linq;

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
