using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SplashScreenViewModel : Screen, IHandle<LoadingMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private string _loadingText;
        public string LoadingText
        {
            get => _loadingText;
            set
            {
                _loadingText = value;
                NotifyOfPropertyChange();
            }
        }

        public SplashScreenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public Task HandleAsync(LoadingMessage message, CancellationToken cancellationToken)
        {
            LoadingText = message.Text;
            if (message.IsComplete)
            {
                return TryCloseAsync();
            }
            return Task.CompletedTask;
        }
    }
}
