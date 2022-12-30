using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager _windowManager = new WindowManager(); // temp

        public MainViewModel MainPanel { get; }
        public FilterOptionViewModel FilterOptionPanel { get; }
        public FilterOptionIndicatorViewModel FilterOptionIndicator { get; }

        public ShellViewModel() // temp
        {
            MainPanel = IoC.Get<MainViewModel>();
            FilterOptionPanel = IoC.Get<FilterOptionViewModel>();
            FilterOptionIndicator = IoC.Get<FilterOptionIndicatorViewModel>();
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
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

        public void OpenReleasePage()
        {
            string url = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
            System.Diagnostics.Process.Start(url);
        }

        public void ShowInfoDialog()
        {
            _windowManager.ShowDialogAsync(new InfoViewModel(0, 0)); // temp
        }

        public void ShowSettingDialog()
        {
            _windowManager.ShowDialogAsync(new SettingViewModel(null)); // temp
        }
    }
}
