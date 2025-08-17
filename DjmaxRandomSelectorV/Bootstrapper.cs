using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
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

        private readonly RandomSelectorService _rs;
        private readonly TrackDB _db;
        private readonly LocatorService _loc;
        private readonly History _history;
        private readonly ConditionManager _condManager;
        private readonly GroupwiseExtractorBuilder _extrBuild;
        private readonly RandomSelectorExecutor _executor;
        private readonly HotKeyService _hotkey;
        private readonly ConfigurationManager _configManager;
        private readonly UpdateManager _updateManager;

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
            _container.Instance(_config); // TODO: delete it (used at ShellVM)
            // Executor Components
            var eventaggregator = IoC.Get<IEventAggregator>();
            _history = new History(_config.RecentPlayed)
            {
                Limit = _config.RecentsCount
            };
            _rs = new RandomSelectorService(_history);
            _rs.OnSelectionCompleted += pattern => eventaggregator.PublishOnUIThreadAsync(new PatternMessage(pattern));
            _db = new TrackDB(_fileManager);
            _loc = new LocatorService()
            {
                InputInterval = _config.InputDelay,
                MusicForm = _config.Mode,
                InputMethod = _config.Aider
            };
            _condManager = new ConditionManager();
            _extrBuild = new GroupwiseExtractorBuilder()
            {
                StyleType = _config.Mode,
                LevelPreference = _config.Level
            };
            // Executor and Hotkey Manager
            _executor = new RandomSelectorExecutor(_rs, _db, _loc, _condManager, _extrBuild);
            _condManager.OnFilterStateChanged += _executor.SetStateChanged;
            _hotkey = new HotKeyService(_executor)
            {
                IgnoreTitleChecker = _config.Aider == InputMethod.NotInput
            };
            // Configuration Manager
            _configManager = new ConfigurationManager(_config);
            _configManager.OnFilterOptionStateChanged += filterOption =>
            {
                _loc.MusicForm = filterOption.Mode;
                _loc.InputMethod = filterOption.Aider;
                _extrBuild.StyleType = filterOption.Mode;
                _extrBuild.LevelPreference = filterOption.Level;
                _history.Limit = filterOption.RecentsCount;
                _executor.SetStateChanged();
                _hotkey.IgnoreTitleChecker = filterOption.Aider == InputMethod.NotInput;
            };
            _configManager.OnSettingStateChanged += setting =>
            {
                _db.SetPlayable(setting.OwnedDlcs);
                _loc.InputInterval = setting.InputDelay;
                _loc.SetLocationMap(_db.AllTrack);
                _executor.SetStateChanged();
                // filemanager export?
            };
            // Etc.
            _updateManager = new UpdateManager(_fileManager, _configManager);
            _container
                .Instance<ITrackDB>(_db)
                .Instance<IFilterStateManager>(_condManager)
                .Instance<IFilterOptionStateManager>(_configManager)
                .Instance<ISettingStateManager>(_configManager)
                .Instance<IReadOnlyVersionInfoStateManager>(_configManager);
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
                await _updateManager.UpdateAsync();
            }
            catch (Exception ex)
            {
                _ = Task.Run(() => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
            var versionInfo = _configManager.GetReadOnlyVersionInfo();
            if (versionInfo.AppdataVersion.CompareTo(_config.AppdataVersion) > 0)
            {
                _ = Task.Run(() => MessageBox.Show($"App data has been updated to the version {versionInfo.AppdataVersion}.",
                             "Update", MessageBoxButton.OK, MessageBoxImage.Information));
            }
            // Import appdata
            Dmrsv3Appdata appdata;
            try
            {
                appdata = _fileManager.Import<Dmrsv3Appdata>(AppDataFilePath);
            }
            catch
            {
                MessageBox.Show("Cannot import appdata.json. Try again or download the file manually.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Exit -= OnExit;
                Application.Shutdown();
                return;
            }
            // Set AllTrack
            _db.Initialize(appdata);
            _db.ImportDB();
            _db.SetPlayable(_config.OwnedDlcs);
            _loc.SetLocationMap(_db.AllTrack);
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
            // Set hotkey manager
            _hotkey.Initialize(window);
            _hotkey.SetHotKey(_config.StartKeyCode);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _config.RecentPlayed = _config.SavesRecents ? _history.ToList() : new List<int>();
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
