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

        public PresetViewModel PresetViewModel { get; set; }
        public FavoriteViewModel FavoriteViewModel { get; set; }

        public InventoryViewModel(Setting setting, DockPanel dockPanel)
        {
            _setting = setting;
            _dockPanel = dockPanel;

            PresetViewModel = new PresetViewModel();
            FavoriteViewModel = new FavoriteViewModel(_setting.Favorite);
        }

        #region Tab
        private bool _isPresetTabSelected = true;
        private bool _isFavoriteTabSelected = false;
        public bool IsPresetTabSelected
        {
            get => _isPresetTabSelected;
            set
            {
                _isPresetTabSelected = value;
                NotifyOfPropertyChange(() => IsPresetTabSelected);
            }
        }
        public bool IsFavoriteTabSelected
        {
            get => _isFavoriteTabSelected;
            set
            {
                _isFavoriteTabSelected = value;
                NotifyOfPropertyChange(() => IsFavoriteTabSelected);
            }
        }
        public void LoadPresetTab() { IsPresetTabSelected = true; }
        public void LoadFavoriteTab() { IsFavoriteTabSelected = true; }
        #endregion

        public void OK()
        {
            Manager.SaveSetting(_setting);
            TryCloseAsync();
            _dockPanel.Effect = null;
        }
    }
}
