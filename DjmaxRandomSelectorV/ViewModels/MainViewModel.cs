using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>.Collection.OneActive, IHandle<SettingMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            var type = GetFilterPanelType(IoC.Get<Configuration>().FilterType);
            ActivateItemAsync(IoC.GetInstance(type, null));
            ActivateItemAsync(IoC.Get<HistoryViewModel>());
            ChangeActiveItemAsync(Items[0], false);
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            var type = GetFilterPanelType(message.FilterType);
            if (type != Items[0].GetType())
            {
                DeactivateItemAsync(Items[0], true, cancellationToken);
                Items.Insert(0, IoC.GetInstance(type, null));
                ActivateItemAsync(Items[0], cancellationToken);
            }
            return Task.CompletedTask;
        }

        private Type GetFilterPanelType(FilterType filterType) => filterType switch
        {
            FilterType.Query => typeof(BasicFilterViewModel),
            FilterType.Playlist => typeof(PlaylistFilterViewModel),
            _ => throw new NotSupportedException(),
        };
    }
}
