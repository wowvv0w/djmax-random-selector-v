using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string ConfigPath = @"Data\Config.json";

        private readonly SimpleContainer _container = new SimpleContainer();
        private readonly Configuration _configuration;
        private readonly RandomSelector _rs;
        private readonly IFileManager _fileManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly CategoryContainer _categoryContainer = new CategoryContainer();

        private VersionContainer _versionContainer;

        public Bootstrapper()
        {
            Initialize();

            _fileManager = IoC.Get<IFileManager>();
            _configuration = _fileManager.Import<Configuration>(ConfigPath);
            _container.Instance(_configuration);

            _eventAggregator = IoC.Get<IEventAggregator>();
            _rs = new RandomSelector(_eventAggregator);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            Task<int[]> version = new UpdateHelper().GetLastestVersionsAsync();
            Task display = DisplayRootViewForAsync(typeof(ShellViewModel));

            await display;
            Window window = Application.MainWindow;
            double[] position = _configuration.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }

            int[] lastest = null;
            try
            {
                lastest = await version;
            }
            catch
            {
                MessageBox.Show("Failed to version check.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            int appVersion = assemblyVersion.Major * 100 +
                             assemblyVersion.Minor * 10 +
                             assemblyVersion.Build;

            if (lastest is not null)
            {
                var current = new int[] { appVersion, _configuration.AllTrackVersion };
                if (current[0] < lastest[0])
                {
                    await _eventAggregator.PublishOnUIThreadAsync(new UpdateMessage());
                }
                if (current[1] != lastest[1])
                {
                    try
                    {
                        new TrackManager().DownloadAllTrack();
                        _configuration.AllTrackVersion = lastest[1];
                        MessageBox.Show($"All track list is updated to the version {lastest[1]}.",
                            "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }
                    catch
                    {
                        MessageBox.Show("Failed to download all track list.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            _rs.Initialize(_configuration);
            _rs.RegisterHandle(new WindowInteropHelper(window).Handle);
            _rs.SetHotkey(0x0000, 118);
            _versionContainer = new VersionContainer
            {
                CurrentAppVersion = appVersion,
                LastestAppVersion = lastest?[0] ?? appVersion,
                AllTrackVersion = _configuration.AllTrackVersion
            };
            _container.Instance(_versionContainer);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var historyItems = _rs.History.GetItems();
            if (_configuration.SavesRecents)
            {
                _configuration.Exclusions = historyItems;
            }
            _fileManager.Export(_configuration, ConfigPath);
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Instance(_categoryContainer);

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
