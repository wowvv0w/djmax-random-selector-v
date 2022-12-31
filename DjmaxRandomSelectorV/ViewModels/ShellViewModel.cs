using Caliburn.Micro;
using Dmrsv.RandomSelector;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly Selector _selector;

        public MainViewModel MainPanel { get; }
        public FilterOptionViewModel FilterOptionPanel { get; }
        public FilterOptionIndicatorViewModel FilterOptionIndicator { get; }

        public ShellViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;

            MainPanel = IoC.Get<MainViewModel>();
            FilterOptionIndicator = IoC.Get<FilterOptionIndicatorViewModel>();
            FilterOptionPanel = IoC.Get<FilterOptionViewModel>();

            _selector = new Selector();
        }

        protected override void OnViewLoaded(object view)
        {
            var window = view as Window;
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(_selector.Starter.HwndHook);
            _selector.Starter.AddHotkey(handle, 9000, 0x0000, 118);
            _selector.Starter.ExecutionFailed += e => MessageBox.Show(e, "Selector Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _selector.Starter.ExecutionComplete += e => _eventAggregator.PublishOnUIThreadAsync(e);
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
            return _windowManager.ShowDialogAsync(IoC.Get<SettingViewModel>()); // temp
        }
    }
}
