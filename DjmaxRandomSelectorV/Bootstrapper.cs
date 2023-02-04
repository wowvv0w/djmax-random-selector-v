using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        private readonly CategoryContainer _categoryContainer = new CategoryContainer();
        private readonly VersionContainer _versionContainer;

        private Mutex _mutex;

        public Bootstrapper()
        {
            Initialize();

            _fileManager = IoC.Get<IFileManager>();
            try
            {
                _configuration = _fileManager.Import<Configuration>(ConfigPath);
            }
            catch
            {
                _configuration = new Configuration();
            }
            _container.Instance(_configuration);

            var eventAggregator = IoC.Get<IEventAggregator>();
            _rs = new RandomSelector(eventAggregator);

            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            int appVersion = assemblyVersion.Major * 100 +
                             assemblyVersion.Minor * 10 +
                             assemblyVersion.Build;
            _versionContainer = new VersionContainer(appVersion, _configuration.AllTrackVersion);
            _versionContainer.NewAllTrackVersionAvailable += (s, e) =>
            {
                try
                {
                    new TrackManager().DownloadAllTrack();
                }
                catch
                {
                    throw new Exception($"Failed to download lastest all track list (Version {e.Version})");
                }
                Task.Run(() =>
                {
                    MessageBox.Show($"All track list is updated to the version {e.Version}.",
                        "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                _configuration.AllTrackVersion = e.Version;
            };
            _container.Instance(_versionContainer);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            _mutex = new Mutex(true, "DjmaxRandomSelectorV", out bool createsNew);
            if (!createsNew)
            {
                MessageBox.Show("Already running.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Exit -= OnExit;
                Application.Shutdown();
                return;
            }

            try
            {
                await _versionContainer.CheckLastestVersionsAsync();
            }
            catch (Exception ex)
            {
                _ = Task.Run(() =>
                {
                    MessageBox.Show(ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }

            await DisplayRootViewForAsync(typeof(ShellViewModel));
            Window window = Application.MainWindow;
            double[] position = _configuration.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }

            _rs.Initialize(_configuration);
            _rs.RegisterHandle(new WindowInteropHelper(window).Handle);
            _rs.SetHotkey(0x0000, 118);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var historyItems = _rs.History.GetItems().ToList();
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
