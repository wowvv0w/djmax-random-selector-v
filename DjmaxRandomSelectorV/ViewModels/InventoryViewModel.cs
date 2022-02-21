using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InventoryViewModel : Screen
    {
        private Setting _setting;
        private DockPanel _dockPanel;

        public FavoriteViewModel FavoriteViewModel { get; set; }

        public InventoryViewModel(Setting setting, DockPanel dockPanel)
        {
            _setting = setting;
            _dockPanel = dockPanel;

            FavoriteViewModel = new FavoriteViewModel(_setting.Favorite);
        }


        public void OK()
        {
            Manager.SaveSetting(_setting);
            TryCloseAsync();
            _dockPanel.Effect = null;
        }
    }
}
