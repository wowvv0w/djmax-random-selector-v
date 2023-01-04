using Caliburn.Micro;
using Dmrsv.Data;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.AllActive
    {
        private readonly IWindowManager _windowManager;

        public object MainPanel { get => Items[0]; }
        public object FilterOptionIndicator { get => Items[1]; }
        public object FilterOptionPanel { get => Items[2]; }

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

        protected override void OnViewLoaded(object view)
        {
            var window = view as Window;

            var selector = IoC.Get<Selector>();
            var executor = new ExecutionHelper(selector.CanStart, selector.Start);
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(executor.HwndHook);
            executor.AddHotkey(handle, 9000, 0x0000, 118);
            //executor.ExecutionFailed += e => MessageBox.Show(e, "Selector Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //executor.ExecutionComplete += e => _eventAggregator.PublishOnUIThreadAsync(e);

            var config = IoC.Get<Configuration>();
            double[] position = config.Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
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
            DeactivateItemAsync(MainPanel, true);
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
