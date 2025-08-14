using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.Extractors;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using DjmaxRandomSelectorV.ViewModels;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper : BootstrapperBase
    {
        private const string AppDataFilePath = @"DMRSV3_Data\appdata.json";
        private const string ConfigFilePath = @"DMRSV3_Data\Config.json";

        private readonly IFileManager _fileManager;
        private readonly SimpleContainer _container = new SimpleContainer();

        private readonly Dmrsv3Configuration _config;
        //private readonly RandomSelectorAPI _rs;
        //private readonly CategoryContainer _categoryContainer;
        //private readonly VersionContainer _versionContainer;
        //private readonly UpdateManager _updater;
        //private readonly ExecutionHelper _executor;

        // NEW
        private readonly RandomSelectorService _rs;
        private readonly TrackDB _db;
        private readonly LocatorService _loc;
        private readonly History _history;
        private readonly ConditionBuilder _condBuild;
        private readonly GroupwiseExtractorBuilder _extrBuild;
        private readonly RandomSelectorExecutor _executor;
        private readonly HotKeyService _hotkey;
        private readonly ConfigurationManager _configManager;
        private readonly UpdateManager _updateManager;
        // END NEW

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

            //_categoryContainer = new CategoryContainer();
            //_container.Instance(_categoryContainer);

            //_db = new TrackDB(_fileManager);
            //_container.Instance(_db);

            // NEW
            var eventaggregator = IoC.Get<IEventAggregator>();
            _history = new History(_config.RecentPlayed)
            {
                Limit = _config.RecentsCount
            };
            _rs = new RandomSelectorService(_history);
            _rs.OnSelectionCompleted += pattern => eventaggregator.PublishOnUIThreadAsync(new PatternMessage(pattern));
            _db = new TrackDB(_fileManager);
            _loc = new LocatorService(); // TODO: complete implementation
            _condBuild = new ConditionBuilder();
            _extrBuild = new GroupwiseExtractorBuilder()
            {
                StyleType = _config.Mode,
                LevelPreference = _config.Level
            };

            _executor = new RandomSelectorExecutor(_rs, _db, _loc, _condBuild, _extrBuild);
            _condBuild.OnFilterStateChanged += _executor.SetStateChanged;
            _hotkey = new HotKeyService(_executor)
            {
                IgnoreTitleChecker = _config.Aider == InputMethod.NotInput
            };

            _configManager = new ConfigurationManager(_config);
            _configManager.OnFilterOptionStateChanged += filterOption =>
            {
                // set locator properties
                _extrBuild.StyleType = filterOption.Mode;
                _extrBuild.LevelPreference = filterOption.Level;
                _history.Limit = filterOption.RecentsCount;
                _executor.SetStateChanged();
                _hotkey.IgnoreTitleChecker = filterOption.Aider == InputMethod.NotInput;
            };
            _configManager.OnSettingStateChanged += setting =>
            {
                _db.SetPlayable(setting.OwnedDlcs);
                // set locator input interval
                // set locator map
                _executor.SetStateChanged();
                // filemanager export?
            };
            _updateManager = new UpdateManager(_fileManager, _configManager);
            _container
                .Instance<ITrackDB>(_db)
                .Instance<IFilterStateManager>(_condBuild)
                .Instance<IFilterOptionStateManager>(_configManager)
                .Instance<ISettingStateManager>(_configManager)
                .Instance<IReadOnlyVersionInfoStateManager>(_configManager);
            // END NEW


            //var eventAggregator = IoC.Get<IEventAggregator>();
            //_rs = new RandomSelectorAPI(eventAggregator, _db);
            //_executor = new ExecutionHelper(eventAggregator);


            //_versionContainer = new VersionContainer();
            //_container.Instance(_versionContainer);
            //_updater = new UpdateManager(_config, _versionContainer, _fileManager);
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
            //if (_versionContainer.AppdataVersion.CompareTo(_config.AppdataVersion) > 0)
            var versionInfo = _configManager.GetReadOnlyVersionInfo();
            if (versionInfo.AppdataVersion.CompareTo(_config.AppdataVersion) > 0)
            {
                _ = Task.Run(() => MessageBox.Show($"App data has been updated to the version {versionInfo.AppdataVersion}.",
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
            //_categoryContainer.SetCategories(appdata);
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
            //_rs.Initialize(_config); // this was for history and locator
            //_executor.Initialize(_rs, _config);
            //_executor.RegisterHandle(new WindowInteropHelper(window).Handle);
            //_executor.SetHotkey(_config.StartKeyCode);
            _hotkey.Initialize(window);
            _hotkey.SetHotKey(_config.StartKeyCode);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            //var historyItems = _rs.History.GetItems().ToList();
            if (_config.SavesRecents)
            {
                //_config.RecentPlayed = historyItems;
                _config.RecentPlayed = _history.ToList();
            }
            //_config.AllTrackVersion = _versionContainer.AllTrackVersion;
            //_config.AppdataVersion = _versionContainer.AppdataVersion;
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
