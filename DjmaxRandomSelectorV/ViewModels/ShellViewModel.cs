using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly IWindowManager _windowManager;

        public object MainPanel { get => Items[0]; }
        public object FilterOptionIndicator { get => Items[1]; }
        public object FilterOptionPanel { get => Items[2]; }

        private Visibility _openReleasePageVisibility;
        public Visibility OpenReleasePageVisibility
        {
            get { return _openReleasePageVisibility; }
            set
            {
                _openReleasePageVisibility = value;
                NotifyOfPropertyChange(() => OpenReleasePageVisibility);
            }
        }

        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

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
            var config = IoC.Get<Configuration>();
            config.Position = new double[2] { window.Top, window.Left };
            window.Close();
        }

        public void OpenReleasePage() // TODO: Fix error
        {
            string url = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
            System.Diagnostics.Process.Start(url);
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
