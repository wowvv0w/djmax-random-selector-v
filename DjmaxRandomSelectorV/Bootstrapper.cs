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
        private SimpleContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IWindowManager, WindowManager>();

            _container.PerRequest<MainViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<MainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
