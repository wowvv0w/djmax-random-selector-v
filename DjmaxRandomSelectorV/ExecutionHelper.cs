using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using Action = System.Action;

namespace DjmaxRandomSelectorV
{
    public class ExecutionHelper : IHandle<FilterOptionMessage>
    {
        private const int WM_HOTKEY = 0x0312;

        private readonly IEventAggregator _eventAggregator;

        private IntPtr _handle;
        private int _hotkeyID;
        private int _hotkeyWithShiftID;
        private uint _keyCode;

        private Func<bool> _canExecute;
        private Action _execute;
        private Action _executeAgain;

        public bool IgnoreCanExecute { get; set; }

        public ExecutionHelper(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public void Initialize(RandomSelector rs, Dmrsv3Configuration config)
        {
            _canExecute = rs.CanStart;
            _execute = rs.Start;
            _executeAgain = rs.StartAgain;
            IgnoreCanExecute = config.Aider == InputMethod.NotInput;
        }

        public void RegisterHandle(IntPtr handle)
        {
            HwndSource source = HwndSource.FromHwnd(handle);
            _handle = handle;
            source.AddHook(HwndHook);
        }

        public void SetHotkey(uint vk)
        {
            _keyCode = vk;
            _hotkeyID = 9000;
            _hotkeyWithShiftID = 9001;
            RegisterHotKey(_handle, _hotkeyID, (uint)KeyModifiers.None, vk);
            RegisterHotKey(_handle, _hotkeyWithShiftID, (uint)KeyModifiers.Alt, vk);
        }

        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            IgnoreCanExecute = message.InputMethod == InputMethod.NotInput;
            return Task.CompletedTask;
        }

        public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && (wParam.ToInt32() == _hotkeyID || wParam.ToInt32() == _hotkeyWithShiftID))
            {
                int vkey = (int)lParam >> 16 & 0xFFFF;
                if (vkey == _keyCode)
                {
                    if (IgnoreCanExecute || _canExecute.Invoke())
                    {
                        var task = (KeyModifiers)((int)lParam & 0xFFFF) == KeyModifiers.Alt
                                   ? Task.Run(() => _executeAgain.Invoke())
                                   : Task.Run(() => _execute.Invoke());
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }
    }
}
