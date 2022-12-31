using Caliburn.Micro;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Threading;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Controller;
using Dmrsv.Data.Interfaces;
using Dmrsv.Data.Enums;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>.Collection.OneActive
    {
        private const int ApplicationVersion = 151;
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";

        //private readonly IEventAggregator _eventAggregator;

        public FilterBaseViewModel FilterPanel { get; }

        public MainViewModel()
        {
            //_eventAggregator = new EventAggregator();
            //_eventAggregator.SubscribeOnUIThread(this);

            //CheckUpdates();
            FilterPanel = IoC.Get<QueryFilterViewModel>();
            FilterPanel.DisplayName = "FILTER";
        }

        protected override void OnViewLoaded(object view)
        {
            ActivateItemAsync(FilterPanel);
            ActivateItemAsync(IoC.Get<HistoryViewModel>());
            ChangeActiveItemAsync(FilterPanel, false);
        }

        private void CheckUpdates()
        {
            try
            {
                using var client = new HttpClient();
                string result = client.GetStringAsync(VersionsUrl).Result;
                string[] versions = result.Split(',');
                var lastestAppVersion = int.Parse(versions[0]);
                var lastestTrackVersion = int.Parse(versions[1]);

                bool hasAppUpdate = lastestAppVersion > ApplicationVersion;
                //OpenReleasePageVisibility = hasAppUpdate ? Visibility.Visible : Visibility.Hidden;

                var appOption = new OptionApi().GetAppOption();
                var currentTrackVersion = appOption.AllTrackVersion;
                bool hasTrackUpdate = lastestTrackVersion != currentTrackVersion;
                if (hasTrackUpdate || !File.Exists("Data/AllTrackList.csv"))
                {
                    new TrackApi().DownloadAllTrackList();
                    appOption.AllTrackVersion = lastestTrackVersion;
                    new OptionApi().SetAppOption(appOption);
                    MessageBox.Show($"All track list is updated to the version {lastestTrackVersion}.",
                        "DJMAX Random Selector V", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //_infoViewModel = new InfoViewModel(ApplicationVersion, lastestAppVersion);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot check available updates. Check the internet connection.",
                    "Selector Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                //_infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion);
            }
        }


        //public Task HandleAsync(IFilter message, CancellationToken cancellationToken)
        //{
        //    _selector.Handle(message);
        //    return Task.CompletedTask;
        //}
        //public Task HandleAsync(FilterOption message, CancellationToken cancellationToken)
        //{
        //    _selector.Handle(message);
        //    return Task.CompletedTask;
        //}
        //public Task HandleAsync(SelectorOption message, CancellationToken cancellationToken)
        //{
        //    _selector.Handle(message);
        //    ChangeFilterView(message.FilterType);
        //    return Task.CompletedTask;
        //}

        private void ChangeFilterView(FilterType filterType)
        {
            SaveFilter(filterType);
            //FilterViewModel = filterType switch
            //{
            //    FilterType.Query => new QueryFilterViewModel(_eventAggregator, _windowManager),
            //    FilterType.Playlist => new PlaylistFilterViewModel(_eventAggregator),
            //    _ => throw new NotSupportedException(),
            //};
        }

        //protected override void OnViewLoaded(object view)
        //{
        //    var window = view as Window;

        

        //    SetPosition(window);
        //}

        //public void OnClosing(object view)
        //{
        //    var window = view as Window;
        //    var api = new OptionApi();
        //    SaveFilter(api.GetSelectorOption().FilterType);
        //    var appOpt = api.GetAppOption();
        //    appOpt.Position = new double[2] { window.Top, window.Left };
        //    api.SetAppOption(appOpt);
        //    api.SaveConfig();
        //}

        private void SaveFilter(FilterType filterType)
        {
            var api = new FilterApi();
            if (filterType == FilterType.Query)
                api.SaveQueryFilter();
            else if (filterType == FilterType.Playlist)
                api.SavePlaylistFilter();
            else
                throw new NotSupportedException();
        }

        //public void SetPosition(Window window)
        //{
        //    var position = new OptionApi().GetAppOption().Position;
        //    if (position?.Length == 2)
        //    {
        //        window.Top = position[0];
        //        window.Left = position[1];
        //    }
        //}

        #region Window Top Bar
        //private Visibility _openReleasePageVisibility;
        //public Visibility OpenReleasePageVisibility
        //{
        //    get { return _openReleasePageVisibility; }
        //    set
        //    {
        //        _openReleasePageVisibility = value;
        //        NotifyOfPropertyChange(() => OpenReleasePageVisibility);
        //    }
        //}
        #endregion
    }
}
