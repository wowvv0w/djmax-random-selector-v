using System.Runtime.InteropServices;

namespace Dmrsv.RandomSelector
{
    public class ExecutionHelper
    {
        private const int WM_HOTKEY = 0x0312;

        private IntPtr _handle;
        private int _hotkeyID;
        private uint _keyCode;

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public bool IgnoreCanExecute { get; set; }

        public ExecutionHelper(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
            IgnoreCanExecute = false;
        }

        public void Register(IntPtr hWnd, int id)
        {
            _handle = hWnd;
            _hotkeyID = id;
        }

        public void SetHotkey(uint fsModifiers, uint vk)
        {
            _keyCode = vk;
            RegisterHotKey(_handle, _hotkeyID, fsModifiers, vk);
        }

        public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _hotkeyID)
            {
                int vkey = (int)lParam >> 16 & 0xFFFF;
                if (vkey == _keyCode)
                {
                    if (IgnoreCanExecute || _canExecute.Invoke())
                    {
                        var task = Task.Run(() => _execute.Invoke());
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    }
}
