using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class Executor
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private const uint KEY_F7 = 118;

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public Executor(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
            AddHotkey();
        }

        private void AddHotkey()
        {
            var window = Application.Current.MainWindow;
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            RegisterHotKey(handle, HOTKEY_ID, 0x0000, KEY_F7);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                int vkey = ((int)lParam >> 16) & 0xFFFF;
                if (vkey == KEY_F7)
                {
                    if (_canExecute.Invoke())
                    {
                        var task = new Task(() => _execute.Invoke());
                        task.Start();
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
