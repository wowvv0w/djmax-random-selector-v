using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV
{
    public class RandomSelector
    {
        public delegate void MusicEventHandler(Music e);
        public event MusicEventHandler OnSelectionSuccessful;

        private IEnumerable<Track> _playable;
        private IEnumerable<Music> _candidates;

        private bool _isUpdated;
        private bool _isRunning;

        private ExecutionHelper _executionHelper;
        private IFilter _filter;
        private IFilterHandler<IFilter> _filterHandler;
        private ISelector _selector;
        private Locator _keyInputInvoker;
        private WindowTitleHelper _windowTitleHelper;

        public ExecutionHelper ExecutionHelper { get => _executionHelper; }
        public IFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                SetUpdated();
            }
        }
        public IFilterHandler<IFilter> FilterHandler
        {
            get => _filterHandler;
            set
            {
                _filterHandler = value;
                SetUpdated();
            }
        }
        public ISelector Selector { get => _selector; }
        public Locator KeyInputInvoker { get => _keyInputInvoker; }

        public RandomSelector()
        {
            _isUpdated = true;
            _isRunning = false;
            var canExecute = () =>
            {
                if (!_windowTitleHelper.EqualsDjmax())
                {
                    ShowErrorMessage("The foreground window is not \"DJMAX RESPECT V\".\nPress start key in the game.");
                    return false;
                }
                if (!_isRunning)
                {
                    return true;
                }

                return false;
            };
            _executionHelper = new ExecutionHelper(canExecute, Start);
            _selector = new SelectorWithRecent();
            _keyInputInvoker = new Locator();
        }

        public void Start()
        {
            _isRunning = true;

            if (_isUpdated)
            {
                _candidates = FilterHandler.Filter(_playable, Filter);
            }

            var selected = Selector.Select(_candidates);

            if (selected is not null)
            {
                KeyInputInvoker.Provide(selected, _playable.ToList());
                OnSelectionSuccessful.Invoke(selected);
            }
            else
            {
                ShowErrorMessage("There is no music that meets the filter conditions.");
            }

            _isRunning = false;
        }

        public void SetUpdated() => _isUpdated = true;

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
