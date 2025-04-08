using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string AppDataFilePath = @"DMRSV3_Data\appdata.json";
        private const string ConfigFilePath = @"DMRSV3_Data\Config.json";
        private const string AppDataDownloadUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DMRSV3_Data/appdata.json";

        private readonly SimpleContainer _container = new SimpleContainer();
        private readonly Dmrsv3AppData _appdata;
        private readonly Dmrsv3Configuration _config;
        private readonly RandomSelector _rs;
        private readonly TrackDB _db;
        private readonly IFileManager _fileManager;
        private readonly CategoryContainer _categoryContainer;
        private readonly UpdateManager _updater;
        private readonly ExecutionHelper _executor;

        private Mutex _mutex;

        public Bootstrapper()
        {
            Initialize();

            _fileManager = IoC.Get<IFileManager>();
            try
            {
                _appdata = _fileManager.Import<Dmrsv3AppData>(AppDataFilePath);
            }
            catch
            {
                /*
                string msg = "Failed to load appdata.json.\nWould you like to download new one?";
                var result = MessageBox.Show(msg, "Notice", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using var client = new HttpClient();
                    string appdata = client.GetStringAsync(AppDataDownloadUrl).Result;
                    using var writer = new StreamWriter(AppDataFilePath);
                    writer.Write(appdata);
                    _appdata = _fileManager.Import<Dmrsv3AppData>(AppDataFilePath);
                }
                */
            }

            try
            {
                _config = _fileManager.Import<Dmrsv3Configuration>(ConfigFilePath);
            }
            catch
            {
                _config = new Dmrsv3Configuration();
            }
            _container.Instance(_config);
            
            _db = new TrackDB(_appdata);
            _container.Instance(_db);

            _categoryContainer = new CategoryContainer(_appdata);
            _container.Instance(_categoryContainer);

            var eventAggregator = IoC.Get<IEventAggregator>();
            _rs = new RandomSelector(eventAggregator);
            _executor = new ExecutionHelper(eventAggregator);

            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            _updater = new UpdateManager(_config, assemblyVersion);
            _container.Instance(_updater);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            // Prevent redundant running
            _mutex = new Mutex(true, "DjmaxRandomSelectorV", out bool createsNew);
            if (!createsNew)
            {
                MessageBox.Show("Already running.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Exit -= OnExit;
                Application.Shutdown();
                return;
            }
            // Update all track file
            try
            {
                bool[] result = _updater.CheckUpdates();
                // If AllTrack update is available
                if (result[1])
                {
                    try
                    {
                        _db.RequestDB();
                    }
                    catch
                    {
                        throw new Exception($"Failed to download lastest all track list (Version {_updater.AllTrackVersion})");
                    }
                    _ = Task.Run(() =>
                    {
                        MessageBox.Show($"All track list is updated to the version {_updater.AllTrackVersion}.",
                            "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    _config.VersionInfo.AllTrackVersion = _updater.AllTrackVersion;
                }
            }
            catch (Exception ex)
            {
                _ = Task.Run(() =>
                {
                    MessageBox.Show(ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            // Bind views and viewmodels
            await DisplayRootViewForAsync(typeof(ShellViewModel));
            // Set window property
            var winProp = _config.WindowProperty;
            Window window = Application.MainWindow;
            double[] position = winProp.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
            // Set random selector
            _db.ImportDB();
            _rs.Initialize(_config);
            _executor.Initialize(_rs.CanStart, _rs.Start, _config);
            _executor.RegisterHandle(new WindowInteropHelper(window).Handle);
            _executor.SetHotkey(0x0000, 118);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var setting = _config.Setting;
            var historyItems = _rs.History.GetItems().ToList();
            if (setting.SavesRecent)
            {
                setting.RecentPlayed = historyItems;
            }
            _fileManager.Export(_config, ConfigFilePath);
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
