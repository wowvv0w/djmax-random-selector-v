using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public object MainPanel { get => Items[0]; }
        public object FilterOptionIndicator { get => Items[1]; }
        public object FilterOptionPanel { get => Items[2]; }
        public Visibility OpenReleasePageVisibility { get; }

        public ShellViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _windowManager = windowManager;

            var container = IoC.Get<VersionContainer>();
            bool visible = container.CurrentAppVersion < container.LatestAppVersion;
            OpenReleasePageVisibility = visible ? Visibility.Visible : Visibility.Hidden;

            var childrenType = new List<Type>()
            {
                typeof(MainViewModel),
                typeof(FilterOptionIndicatorViewModel),
                typeof(FilterOptionViewModel)
            };
            childrenType.ForEach(type => ActivateItemAsync(IoC.GetInstance(type, null)));
        }

        public void MoveWindow(object view)
        {
            var window = view as Window;
            window.DragMove();
        }

        public void MinimizeWindow(object view)
        {
            var window = view as Window;
            window.WindowState = WindowState.Minimized;
        }

        public void CloseWindow(object view)
        {
            var window = view as Window;
            var config = IoC.Get<Dmrsv3Configuration>();
            config.Position = new double[2] { window.Top, window.Left };
            window.Close();
        }

        public void OpenReleasePage()
        {
            string url = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
            System.Diagnostics.Process.Start("explorer.exe", url);
        }

        public Task ShowInfoDialog()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<InfoViewModel>());
        }

        public Task ShowSettingDialog()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<SettingViewModel>());
        }
    }
}
