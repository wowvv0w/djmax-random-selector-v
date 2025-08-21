using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DjmaxRandomSelectorV
{
    public class Bootstrapper
    {
        private readonly IFileManager _fileManager;

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
            _fileManager = new FileManager();

            try
            {
                _config = _fileManager.Import<Dmrsv3Configuration>(DmrsvPath.ConfigFile);
            }
            catch
            {
                _config = new Dmrsv3Configuration();
            }

            // Executor Components
            _history = new History(_config.RecentPlayed)
            {
                Limit = _config.RecentsCount
            };
            _rs = new RandomSelectorService(_history);
            // TODO: migrate it
            //var eventaggregator = IoC.Get<IEventAggregator>();
            //_rs.OnSelectionCompleted += pattern => eventaggregator.PublishOnUIThreadAsync(new PatternMessage(pattern));
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
        }

        public async Task Startup()
        {
            // Prevent redundant running
            _mutex = new Mutex(true, "DjmaxRandomSelectorV", out bool createdNew);
            if (!createdNew)
            {
                throw new Exception("Already running.");
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
                appdata = _fileManager.Import<Dmrsv3Appdata>(DmrsvPath.AppdataFile);
            }
            catch
            {
                throw new Exception("Cannot import appdata.json. Try again or download the file manually.");
            }

            // Set AllTrack
            _db.Initialize(appdata);
            _db.ImportDB();
            _db.SetPlayable(_config.OwnedDlcs);
            _loc.SetLocationMap(_db.AllTrack);

            // Create service provider
            var services
                = new ServiceCollection()
                .AddSingleton(_fileManager)
                .AddSingleton(_config)// TODO: delete it (used at ShellVM)
                .AddSingleton<ITrackDB>(_db)
                .AddSingleton<IFilterStateManager>(_condManager)
                .AddSingleton<IFilterOptionStateManager>(_configManager)
                .AddSingleton<ISettingStateManager>(_configManager)
                .AddSingleton<IReadOnlyVersionInfoStateManager>(_configManager);

            Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => services.AddTransient(viewModelType));

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            // Show main window
            var window = new ShellView();
            double[] position = _config.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
            window.Show();

            // Set hotkey manager
            _hotkey.Initialize(window);
            _hotkey.SetHotKey(_config.StartKeyCode);
        }

        public void Exit()
        {
            _config.RecentPlayed = _config.SavesRecents ? _history.ToList() : new List<int>();
            _fileManager.Export(_config, DmrsvPath.ConfigFile);
        }
    }
}
