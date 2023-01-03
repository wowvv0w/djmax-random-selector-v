using Dmrsv.Data;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Dmrsv.RandomSelector
{
    public class ExecutionHelper
    {
        private const int WM_HOTKEY = 0x0312;

        private int _hotkeyID;
        private uint _keyCode;

        private readonly Func<bool> _canExecute;
        private readonly Func<Music> _execute;

        //public delegate void MessageEventHandler(string e);
        //public delegate void MusicEventHandler(Music e);
        //public event MessageEventHandler? ExecutionFailed;
        //public event MusicEventHandler? ExecutionComplete;

        public ExecutionHelper(Func<bool> canExecute, Func<Music> execute)
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

        public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _hotkeyID)
            {
                int vkey = (int)lParam >> 16 & 0xFFFF;
                if (vkey == _keyCode)
                {
                    //try
                    //{
                    if (_canExecute.Invoke())
                    {
                        var task = Task.Run(() => _execute.Invoke());
                    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    ExecutionFailed?.Invoke(ex.Message);
                    //}
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    }
}
