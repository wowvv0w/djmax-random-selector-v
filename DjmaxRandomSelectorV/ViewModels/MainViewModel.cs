using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Services;

namespace DjmaxRandomSelectorV.ViewModels
{
    public partial class MainViewModel : ObservableRecipient, IRecipient<FilterTypeChangedMessage>
    {
        //private readonly IEventAggregator _eventAggregator;
        public ObservableCollection<object> Items { get; } = new();
        public object ActivatedItem { get; } = null;

        public class TempMock
        {
            public string DisplayName { get; set; }
        }
        public MainViewModel(ISettingStateManager settingManager)
        {
            //_eventAggregator = eventAggregator;
            //_eventAggregator.SubscribeOnUIThread(this);

            var type = GetFilterPanelType(settingManager.GetSetting().FilterType);
            var temp = new TempMock() { DisplayName = "TEST" };
            Items.Add(temp);
            //ActivateItemAsync(IoC.GetInstance(type, null));
            //ActivateItemAsync(IoC.Get<HistoryViewModel>());
            //ChangeActiveItemAsync(Items[0], false);
        }

        public Task HandleAsync(FilterTypeChangedMessage message, CancellationToken cancellationToken)
        {
            //var type = GetFilterPanelType(message.FilterType);
            //if (type != Items[0].GetType())
            //{
            //    DeactivateItemAsync(Items[0], true, cancellationToken);
            //    Items.Insert(0, IoC.GetInstance(type, null));
            //    ActivateItemAsync(Items[0], cancellationToken);
            //}
            return Task.CompletedTask;
        }

        public void Receive(FilterTypeChangedMessage message)
        {
            throw new NotImplementedException();
        }

        private Type GetFilterPanelType(FilterType filterType) => filterType switch
        {
            FilterType.Query => typeof(BasicFilterViewModel),
            FilterType.Playlist => typeof(AdvancedFilterViewModel),
            _ => throw new NotSupportedException(),
        };
    }
}
