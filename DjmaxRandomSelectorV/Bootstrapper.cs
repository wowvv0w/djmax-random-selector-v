using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.Data;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows;
using System.Xml.Linq;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string ConfigPath = @"Data\config.json";

        private readonly SimpleContainer _container = new SimpleContainer();
        private readonly Configuration _configuration = new FileManager().Import<Configuration>(ConfigPath);
        private readonly Selector _selector = new Selector();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(ShellViewModel));
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            new FileManager().Export(_configuration, ConfigPath);
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Instance(_configuration);
            _container.Instance(_selector);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IFileManager, FileManager>();

            foreach (var assembly in SelectAssemblies())
            {
                assembly.GetTypes()
                    .Where(type => type.IsClass)
                    .Where(type => type.Name.EndsWith("ViewModel"))
                    .ToList()
                    .ForEach(viewModelType => _container.RegisterPerRequest(
                        viewModelType, viewModelType.ToString(), viewModelType));
            }
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
