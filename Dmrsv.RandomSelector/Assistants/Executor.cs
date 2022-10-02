using System.Runtime.InteropServices;

namespace Dmrsv.RandomSelector.Assistants
{
    public class Executor
    {
        private const int WM_HOTKEY = 0x0312;

        private int _hotkeyID;
        private uint _keyCode;

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public Executor(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public void AddHotkey(IntPtr hWnd, int id, uint fsModifiers, uint vk)
        {
            _hotkeyID = id;
            _keyCode = vk;
            RegisterHotKey(hWnd, id, fsModifiers, vk);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _hotkeyID)
            {
                int vkey = (int)lParam >> 16 & 0xFFFF;
                if (vkey == _keyCode)
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
