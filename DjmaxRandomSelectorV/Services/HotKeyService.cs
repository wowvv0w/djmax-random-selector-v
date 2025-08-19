using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV.Services
{
    public class HotKeyService
    {
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_NONE = 0x0000;
        private const uint MOD_ALT = 0x0001;
        //private const uint MOD_CONTROL = 0x0002;
        //private const uint MOD_SHIFT = 0x0004;
        //private const uint MOD_WIN = 0x0008;

        private const int StartHotKeyId = 9000;
        private const int RestartHotKeyId = 9001;

        private readonly IExecutable _executor;
        private readonly ForegroundWindowTitleChecker _titleChecker;

        private IntPtr _handle = IntPtr.Zero;
        private uint _keyCode = 0x00;

        public bool IgnoreTitleChecker { get; set; }

        public HotKeyService(IExecutable executor)
        {
            _executor = executor;
            _executor.OnExecutionCompleted += result =>
            {
                if (result == false)
                {
                    MessageBox.Show("There is no music that meets the filter conditions.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            };
            _titleChecker = new ForegroundWindowTitleChecker("DJMAX RESPECT V");
        }

        public void Initialize(Window mainWindow)
        {
            _handle = new WindowInteropHelper(mainWindow).Handle;
            HwndSource source = HwndSource.FromHwnd(_handle);
            source.AddHook(WndProc);
        }

        public void SetHotKey(uint keyCode)
        {
            _keyCode = keyCode;
            RegisterHotKey(_handle, StartHotKeyId, MOD_NONE, _keyCode);
            RegisterHotKey(_handle, RestartHotKeyId, MOD_ALT, _keyCode);
        }

        public void ResetHotKey()
        {
            UnregisterHotKey(_handle, StartHotKeyId);
            UnregisterHotKey(_handle, RestartHotKeyId);
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && (wParam.ToInt32() == StartHotKeyId || wParam.ToInt32() == RestartHotKeyId))
            {
                int vkey = (int)lParam >> 16 & 0xFFFF;
                if (vkey == _keyCode)
                {
                    if (!_executor.IsRunning)
                    {
                        System.Diagnostics.Debug.WriteLine("execution start");
                        if (IgnoreTitleChecker || _titleChecker.IsAvailable())
                        {
                            System.Diagnostics.Debug.WriteLine("selection executed");
                            var task = ((int)lParam & 0xFFFF) == MOD_ALT
                                       ? Task.Run(() => _executor.Restart())
                                       : Task.Run(() => _executor.Start());
                        }
                        else
                        {
                            var task = Task.Run(() =>
                            {
                                MessageBox.Show("The foreground window is not \"DJMAX RESPECT V\".\nPress start key in the game.",
                                                "Error",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            });
                        }
                        System.Diagnostics.Debug.WriteLine("execution end");
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private class ForegroundWindowTitleChecker
        {
            private readonly string _targetTitle;

            public ForegroundWindowTitleChecker(string targetTitle)
            {
                _targetTitle = targetTitle;
            }

            public bool IsAvailable()
            {
                IntPtr handle = GetForegroundWindow();
                int length = GetWindowTextLength(handle) + 1;
                var buffer = new StringBuilder(length);
                if (GetWindowText(handle, buffer, length) > 0)
                {
                    return buffer.ToString().Equals(_targetTitle);
                }
                return false;
            }

            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll")]
            private static extern int GetWindowTextLength(IntPtr hWnd);
            [DllImport("user32.dll")]
            private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        }
    }
}
