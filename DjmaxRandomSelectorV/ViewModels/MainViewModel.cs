using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>.Collection.OneActive, IHandle<SettingMessage>
    {
        private const int ApplicationVersion = 151;
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";

        private readonly IEventAggregator _eventAggregator;

        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            //CheckUpdates();
            var type = GetFilterPanelType(IoC.Get<Configuration>().FilterType);
            ActivateItemAsync(IoC.GetInstance(type, null));
            ActivateItemAsync(IoC.Get<HistoryViewModel>());
            ChangeActiveItemAsync(Items[0], false);
        }

        //private void CheckUpdates()
        //{
        //    try
        //    {
        //        using var client = new HttpClient();
        //        string result = client.GetStringAsync(VersionsUrl).Result;
        //        string[] versions = result.Split(',');
        //        var lastestAppVersion = int.Parse(versions[0]);
        //        var lastestTrackVersion = int.Parse(versions[1]);

        //        bool hasAppUpdate = lastestAppVersion > ApplicationVersion;
        //        //OpenReleasePageVisibility = hasAppUpdate ? Visibility.Visible : Visibility.Hidden;

        //        var appOption = new OptionApi().GetAppOption();
        //        var currentTrackVersion = appOption.AllTrackVersion;
        //        bool hasTrackUpdate = lastestTrackVersion != currentTrackVersion;
        //        if (hasTrackUpdate || !File.Exists("Data/AllTrackList.csv"))
        //        {
        //            new TrackApi().DownloadAllTrackList();
        //            appOption.AllTrackVersion = lastestTrackVersion;
        //            new OptionApi().SetAppOption(appOption);
        //            MessageBox.Show($"All track list is updated to the version {lastestTrackVersion}.",
        //                "DJMAX Random Selector V", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }

        //        //_infoViewModel = new InfoViewModel(ApplicationVersion, lastestAppVersion);
        //    }
        //    catch (HttpRequestException)
        //    {
        //        MessageBox.Show("Cannot check available updates. Check the internet connection.",
        //            "Selector Error", MessageBoxButton.OK, MessageBoxImage.Warning);

        //        //_infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion);
        //    }
        //}

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            var type = GetFilterPanelType(message.FilterType);
            if (type != Items[0].GetType())
            {
                DeactivateItemAsync(Items[0], true, cancellationToken);
                Items.Insert(0, IoC.GetInstance(type, null));
                ActivateItemAsync(Items[0], cancellationToken);
            }
            return Task.CompletedTask;
        }

        private Type GetFilterPanelType(FilterType filterType) => filterType switch
        {
            FilterType.Query => typeof(QueryFilterViewModel),
            FilterType.Playlist => typeof(PlaylistFilterViewModel),
            _ => throw new NotSupportedException(),
        };
    }
}
