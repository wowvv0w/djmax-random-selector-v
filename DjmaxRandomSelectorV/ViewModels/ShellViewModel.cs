using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await EditFilter();
        }

        public async Task EditFilter()
        {
            var viewmodel = IoC.Get<MainViewModel>();
            await ActivateItemAsync(viewmodel, new CancellationToken());
        }
    }
}
