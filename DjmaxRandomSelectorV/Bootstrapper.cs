using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.Data.Context.Schema;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string ConfigPath = @"Data\config.json";

        private readonly SimpleContainer _container = new SimpleContainer();
        private readonly Configuration _configuration;
        private readonly RandomSelector _rs;
        private readonly IFileManager _fileManager;

        public Bootstrapper()
        {
            Initialize();

            _fileManager = IoC.Get<IFileManager>();
            _configuration = _fileManager.Import<Configuration>(ConfigPath);
            _container.Instance(_configuration);

            _rs = new RandomSelector(IoC.Get<IEventAggregator>());
            _rs.Initialize(_configuration);
            _container.Instance(_rs);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(ShellViewModel));
            Window window = Application.Current.MainWindow;
            _rs.RegisterHandle(new WindowInteropHelper(window).Handle);
            _rs.SetHotkey(0x0000, 118);
            double[] position = _configuration.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _fileManager.Export(_configuration, ConfigPath);
        }

        protected override void Configure()
        {
            _container.Instance(_container);

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
