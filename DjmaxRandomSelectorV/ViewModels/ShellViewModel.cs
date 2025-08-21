using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;

namespace DjmaxRandomSelectorV.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        //private readonly IEventAggregator _eventAggregator;
        //private readonly IWindowManager _windowManager;

        private ObservableCollection<object> Items;
        public object MainPanel { get => Items[0]; }
        public object FilterOptionIndicator { get => Items[1]; }
        public object FilterOptionPanel { get => Items[2]; }
        public Visibility OpenReleasePageVisibility { get; }

        public ShellViewModel(IReadOnlyVersionInfoStateManager versionInfoManager)
        {
            //_eventAggregator = eventAggregator;
            //_eventAggregator.SubscribeOnUIThread(this);
            //_windowManager = windowManager;

            var versionInfo = versionInfoManager.GetReadOnlyVersionInfo();
            bool visible = versionInfo.CurrentAppVersion < versionInfo.LatestAppVersion;
            OpenReleasePageVisibility = visible ? Visibility.Visible : Visibility.Hidden;

            var childrenType = new List<Type>()
            {
                typeof(MainViewModel),
                typeof(FilterOptionIndicatorViewModel),
                typeof(FilterOptionViewModel)
            };
            //childrenType.ForEach(type => ActivateItemAsync(IoC.GetInstance(type, null)));
        }

        [RelayCommand]
        private void OpenReleasePage()
        {
            string url = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
            System.Diagnostics.Process.Start("explorer.exe", url);
        }

        //public Task ShowInfoDialog()
        //{
        //    return _windowManager.ShowDialogAsync(IoC.Get<InfoViewModel>());
        //}

        //public Task ShowSettingDialog()
        //{
        //    return _windowManager.ShowDialogAsync(IoC.Get<SettingViewModel>());
        //}
    }
}
