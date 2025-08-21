using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;

namespace DjmaxRandomSelectorV.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        public object MainPanel { get; }
        public object FilterOptionIndicator { get; }
        public object FilterOptionPanel { get; }
        public Visibility OpenReleasePageVisibility { get; }

        public ShellViewModel(IReadOnlyVersionInfoStateManager versionInfoManager)
        {
            MainPanel = Ioc.Default.GetRequiredService<MainViewModel>();
            //FilterOptionIndicator = Ioc.Default.GetRequiredService<FilterOptionIndicatorViewModel>();
            FilterOptionPanel = Ioc.Default.GetRequiredService<FilterOptionViewModel>();

            var versionInfo = versionInfoManager.GetReadOnlyVersionInfo();
            bool visible = versionInfo.CurrentAppVersion < versionInfo.LatestAppVersion;
            OpenReleasePageVisibility = visible ? Visibility.Visible : Visibility.Hidden;
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
