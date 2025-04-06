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
        private uint _keyCode;

        private Func<bool> _canExecute;
        private Action _execute;

        public bool IgnoreCanExecute { get; set; }

        public ExecutionHelper(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public void Initialize(Func<bool> canExecute, Action execute, Dmrsv3Configuration config)
        {
            _canExecute = canExecute;
            _execute = execute;
            var filterOption = config.FilterOption;
            IgnoreCanExecute = filterOption.InputMethod == InputMethod.NotInput;
        }

        public void RegisterHandle(IntPtr handle)
        {
            HwndSource source = HwndSource.FromHwnd(handle);
            _handle = handle;
            _hotkeyID = 9000;
            source.AddHook(HwndHook);
        }

        public void SetHotkey(uint fsModifiers, uint vk)
        {
            _keyCode = vk;
            RegisterHotKey(_handle, _hotkeyID, fsModifiers, vk);
        }

        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            IgnoreCanExecute = message.InputMethod == InputMethod.NotInput;
            return Task.CompletedTask;
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
