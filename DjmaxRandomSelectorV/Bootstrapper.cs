using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using DjmaxRandomSelectorV.ViewModels;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string AppDataFilePath = @"DMRSV3_Data\appdata.json";
        private const string ConfigFilePath = @"DMRSV3_Data\Config.json";

        private readonly IFileManager _fileManager;
        private readonly SimpleContainer _container = new SimpleContainer();

        private readonly Dmrsv3Configuration _config;
        private readonly RandomSelector _rs;
        private readonly TrackDB _db;
        private readonly CategoryContainer _categoryContainer;
        private readonly VersionContainer _versionContainer;
        private readonly UpdateManager _updater;
        private readonly ExecutionHelper _executor;

        private Mutex _mutex;

        public Bootstrapper()
        {
            Initialize();
            _fileManager = IoC.Get<IFileManager>();

            try
            {
                _config = _fileManager.Import<Dmrsv3Configuration>(ConfigFilePath);
            }
            catch
            {
                _config = new Dmrsv3Configuration();
            }
            _container.Instance(_config);
            
            _categoryContainer = new CategoryContainer();
            _container.Instance(_categoryContainer);

            _db = new TrackDB(_fileManager);
            _container.Instance(_db);

            var eventAggregator = IoC.Get<IEventAggregator>();
            _rs = new RandomSelector(eventAggregator, _db);
            _executor = new ExecutionHelper(eventAggregator);


            _versionContainer = new VersionContainer();
            _container.Instance(_versionContainer);
            _updater = new UpdateManager(_config, _versionContainer, _fileManager);
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
                await _updater.UpdateAsync();
            }
            catch (Exception ex)
            {
                _ = Task.Run(() => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
            if (_versionContainer.AppdataVersion.CompareTo(_config.AppdataVersion) > 0)
            {
                _ = Task.Run(() => MessageBox.Show($"App data has been updated to the version {_versionContainer.AppdataVersion}.",
                             "Update", MessageBoxButton.OK, MessageBoxImage.Information));
            }
            // Import appdata
            Dmrsv3AppData appdata;
            try
            {
                appdata = _fileManager.Import<Dmrsv3AppData>(AppDataFilePath);
            }
            catch
            {
                MessageBox.Show("Cannot import appdata.json. Try again or download the file manually.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Exit -= OnExit;
                Application.Shutdown();
                return;
            }
            _categoryContainer.SetCategories(appdata);
            // Set AllTrack
            _db.Initialize(appdata);
            _db.ImportDB();
            _db.SetPlayable(_config.OwnedDlcs);
            // Bind views and viewmodels
            await DisplayRootViewForAsync(typeof(ShellViewModel));
            // Set window property
            Window window = Application.MainWindow;
            double[] position = _config.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
            // Set random selector
            _rs.Initialize(_config);
            _executor.Initialize(_rs.CanStart, _rs.Start, _config);
            _executor.RegisterHandle(new WindowInteropHelper(window).Handle);
            _executor.SetHotkey(0x0000, 118);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var historyItems = _rs.History.GetItems().ToList();
            if (_config.SavesRecents)
            {
                _config.RecentPlayed = historyItems;
            }
            _config.AllTrackVersion = _versionContainer.AllTrackVersion;
            _config.AppdataVersion = _versionContainer.AppdataVersion;
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
