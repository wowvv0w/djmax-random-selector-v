using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private Setting _setting;
        private DockPanel _dockPanel;

        private int _inputDelay;
        private bool _savesRecents;
        private List<string> _ownedDlcs;

        private bool _updatesTrackList = false;

        public SettingViewModel(Setting setting, DockPanel dockPanel)
        {
            _setting = setting;

            _inputDelay = setting.InputDelay;
            _savesRecents = setting.SavesRecents;
            _ownedDlcs = setting.OwnedDlcs.ConvertAll(x => x);

            UpdateInputDelayText();

            _dockPanel = dockPanel;
        }

        #region On Exit
        public void Apply()
        {
            _setting.InputDelay = _inputDelay;
            _setting.SavesRecents = _savesRecents;
            _setting.OwnedDlcs = _ownedDlcs;

            Manager.SaveSetting(_setting);
            if (_updatesTrackList)
            {
                Manager.UpdateTrackList(_ownedDlcs);
            }
            Selector.IsFilterChanged = true;
            Close();
        }
        public void Cancel()
        {
            Close();
        }
        public void Close()
        {
            TryCloseAsync();
            _dockPanel.Effect = null;
        }
        #endregion

        #region Selector Setting
        public int InputDelay
        {
            get { return _inputDelay; }
            set
            {
                _inputDelay = value;
                NotifyOfPropertyChange(() => InputDelay);
                UpdateInputDelayText();
            }
        }
        private string _inputDelayText;
        public string InputDelayText
        {
            get { return _inputDelayText; }
            set
            {
                _inputDelayText = value;
                NotifyOfPropertyChange(() => InputDelayText);
            }
        }
        public void UpdateInputDelayText()
        {
            InputDelayText = $"{InputDelay}ms";
        }

        public bool SavesRecents
        {
            get { return _savesRecents; }
            set
            {
                _savesRecents = value;
                NotifyOfPropertyChange(() => SavesRecents);
            }
        }
        #endregion

        #region Track List Setting Updater
        public bool CheckOwnedDlcs(string value)
        {
            return _ownedDlcs.Contains(value);
        }
        public void UpdateOwnedDlcs(bool isChecked, string value)
        {
            if (isChecked)
            {
                _ownedDlcs.Add(value);
            }
            else
            {
                _ownedDlcs.Remove(value);
            }
            _updatesTrackList = true;
        }
        #endregion

        #region Track List Setting Elements
        #region Constants
        private const string _P3 = "P3";
        private const string _TR = "TR";
        private const string _CE = "CE";
        private const string _BS = "BS";
        private const string _VE = "VE";
        private const string _VE2 = "VE2";
        private const string _ES = "ES";
        private const string _T1 = "T1";
        private const string _T2 = "T2";
        private const string _T3 = "T3";
        private const string _CHU = "CHU";
        private const string _CY = "CY";
        private const string _DM = "DM";
        private const string _ESTI = "ESTI";
        private const string _GC = "GC";
        private const string _GF = "GF";
        private const string _NXN = "NXN";
        #endregion
        #region Setting
        public bool SettingP3
        {
            get { return CheckOwnedDlcs(_P3); }
            set
            {
                UpdateOwnedDlcs(value, _P3);
                NotifyOfPropertyChange(() => SettingP3);
            }
        }
        public bool SettingTR
        {
            get { return CheckOwnedDlcs(_TR); }
            set
            {
                UpdateOwnedDlcs(value, _TR);
                NotifyOfPropertyChange(() => SettingTR);
            }
        }
        public bool SettingCE
        {
            get { return CheckOwnedDlcs(_CE); }
            set
            {
                UpdateOwnedDlcs(value, _CE);
                NotifyOfPropertyChange(() => SettingCE);
            }
        }
        public bool SettingBS
        {
            get { return CheckOwnedDlcs(_BS); }
            set
            {
                UpdateOwnedDlcs(value, _BS);
                NotifyOfPropertyChange(() => SettingBS);
            }
        }
        public bool SettingVE
        {
            get { return CheckOwnedDlcs(_VE); }
            set
            {
                UpdateOwnedDlcs(value, _VE);
                NotifyOfPropertyChange(() => SettingVE);
            }
        }
        public bool SettingVE2
        {
            get { return CheckOwnedDlcs(_VE2); }
            set
            {
                UpdateOwnedDlcs(value, _VE2);
                NotifyOfPropertyChange(() => SettingVE2);
            }
        }
        public bool SettingES
        {
            get { return CheckOwnedDlcs(_ES); }
            set
            {
                UpdateOwnedDlcs(value, _ES);
                NotifyOfPropertyChange(() => SettingES);
            }
        }
        public bool SettingT1
        {
            get { return CheckOwnedDlcs(_T1); }
            set
            {
                UpdateOwnedDlcs(value, _T1);
                NotifyOfPropertyChange(() => SettingT1);
            }
        }
        public bool SettingT2
        {
            get { return CheckOwnedDlcs(_T2); }
            set
            {
                UpdateOwnedDlcs(value, _T2);
                NotifyOfPropertyChange(() => SettingT2);
            }
        }
        public bool SettingT3
        {
            get { return CheckOwnedDlcs(_T3); }
            set
            {
                UpdateOwnedDlcs(value, _T3);
                NotifyOfPropertyChange(() => SettingT3);
            }
        }
        public bool SettingCHU
        {
            get { return CheckOwnedDlcs(_CHU); }
            set
            {
                UpdateOwnedDlcs(value, _CHU);
                NotifyOfPropertyChange(() => SettingCHU);
            }
        }
        public bool SettingCY
        {
            get { return CheckOwnedDlcs(_CY); }
            set
            {
                UpdateOwnedDlcs(value, _CY);
                NotifyOfPropertyChange(() => SettingCY);
            }
        }
        public bool SettingDM
        {
            get { return CheckOwnedDlcs(_DM); }
            set
            {
                UpdateOwnedDlcs(value, _DM);
                NotifyOfPropertyChange(() => SettingDM);
            }
        }
        public bool SettingESTI
        {
            get { return CheckOwnedDlcs(_ESTI); }
            set
            {
                UpdateOwnedDlcs(value, _ESTI);
                NotifyOfPropertyChange(() => SettingESTI);
            }
        }
        public bool SettingGC
        {
            get { return CheckOwnedDlcs(_GC); }
            set
            {
                UpdateOwnedDlcs(value, _GC);
                NotifyOfPropertyChange(() => SettingGC);
            }
        }
        public bool SettingGF
        {
            get { return CheckOwnedDlcs(_GF); }
            set
            {
                UpdateOwnedDlcs(value, _GF);
                NotifyOfPropertyChange(() => SettingGF);
            }
        }
        public bool SettingNXN
        {
            get { return CheckOwnedDlcs(_NXN); }
            set
            {
                UpdateOwnedDlcs(value, _NXN);
                NotifyOfPropertyChange(() => SettingNXN);
            }
        }
        #endregion
        #endregion

    }
}
