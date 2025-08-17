using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Services;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>.Collection.OneActive, IHandle<FilterTypeChangedMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public MainViewModel(IEventAggregator eventAggregator, ISettingStateManager settingManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            var type = GetFilterPanelType(settingManager.GetSetting().FilterType);
            ActivateItemAsync(IoC.GetInstance(type, null));
            ActivateItemAsync(IoC.Get<HistoryViewModel>());
            ChangeActiveItemAsync(Items[0], false);
        }

        public Task HandleAsync(FilterTypeChangedMessage message, CancellationToken cancellationToken)
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
            FilterType.Playlist => typeof(AdvancedFilterViewModel),
            _ => throw new NotSupportedException(),
        };
    }
}
