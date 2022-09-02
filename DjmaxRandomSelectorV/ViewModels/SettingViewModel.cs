using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using DjmaxRandomSelectorV.Utilities;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private readonly Config _setting;
        private readonly Action<bool> _blurSetter;
        private readonly Action<List<string>> _trackListUpdater;
        private readonly Action<bool> _typeOfFilterSetter;

        private bool _isPlaylist;
        private int _inputDelay;
        private bool _savesRecents;
        private List<string> _ownedDlcs;

        private bool _updatesTrackList = false;

        public SettingViewModel(Config setting, Action<bool> blurSetter, Action<List<string>> trackListUpdater, Action<bool> typeOfFilterSetter)
        {
            _setting = setting;
            _blurSetter = blurSetter;
            _trackListUpdater = trackListUpdater;
            _typeOfFilterSetter = typeOfFilterSetter;

            _isPlaylist = _setting.IsPlaylist;
            _inputDelay = _setting.InputDelay;
            _savesRecents = _setting.SavesRecents;
            _ownedDlcs = _setting.OwnedDlcs.ConvertAll(x => x);

            UpdateInputDelayText();
        }

        #region Detect DLCs
        public void DetectDlcs()
        {
            _ownedDlcs.Clear();

            string steamKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam";
            string steamPath = Registry.GetValue(steamKeyName, "InstallPath", null).ToString();
            
            DirectoryInfo libraryPath = new DirectoryInfo($"{steamPath}\\appcache\\librarycache");

            int dlcCount = 0;
            foreach (FileInfo file in libraryPath.GetFiles())
            {
                if (file.Extension.ToLower().Equals(".jpg"))
                {
                    string dlc = FindDlcs(file.Name.Substring(0, file.Name.Length - 11));
                    if (!string.IsNullOrEmpty(dlc))
                    {
                        _ownedDlcs.Add(dlc);
                        dlcCount++;
                    }
                }
            }
            NotifyOfPropertyChange(string.Empty);
            
            MessageBox.Show($"{dlcCount} DLCs are detected.",
                "Notice",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public string FindDlcs(string name)
        {
            switch (name)
            {
                case "1568680": return _P3;
                case "1271670": return _TR;
                case "1300210": return _CE;
                case "1300211": return _BS;
                case "1080550": return _VE;
                case "1843020": return _VE2;
                case "1238760": return _ES;
                case "1386610": return _T1;
                case "1386611": return _T2;
                case "1386612": return _T3;
                case "1958171": return _TQ;
                case "1472190": return _CHU;
                case "1356221": return _CY;
                case "1356220": return _DM;
                case "1664550": return _ESTI;
                case "1271671": return _GC;
                case "1472191": return _GF;
                case "1782170": return _NXN;
                case "1958170": return _MD;
                default: return null;
            }
        }
        #endregion

        #region On Exit
        public void Apply()
        {
            _setting.InputDelay = _inputDelay;
            _setting.SavesRecents = _savesRecents;
            _setting.OwnedDlcs = _ownedDlcs;
            _setting.IsPlaylist = _isPlaylist;

            FileManager.Export(_setting, "Data/Config.json");
            if (_updatesTrackList)
            {
                _trackListUpdater.Invoke(_ownedDlcs);
            }
            
            _typeOfFilterSetter.Invoke(_isPlaylist);
            Close();
        }

        public void Cancel()
        {
            Close();
        }
        public void Close()
        {
            TryCloseAsync();
            _blurSetter.Invoke(false);
        }
        #endregion

        #region Selector Setting
        public bool IsPlaylist
        {
            get { return _isPlaylist; }
            set
            {
                _isPlaylist = value;
                NotifyOfPropertyChange(() => IsPlaylist);
            }
        }
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
        private const string _TQ = "TQ";
        private const string _CHU = "CHU";
        private const string _CY = "CY";
        private const string _DM = "DM";
        private const string _ESTI = "ESTI";
        private const string _GC = "GC";
        private const string _GF = "GF";
        private const string _MD = "MD";
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
        public bool SettingTQ
        {
            get { return CheckOwnedDlcs(_TQ); }
            set
            {
                UpdateOwnedDlcs(value, _TQ);
                NotifyOfPropertyChange(() => SettingTQ);
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
        public bool SettingMD
        {
            get { return CheckOwnedDlcs(_MD); }
            set
            {
                UpdateOwnedDlcs(value, _MD);
                NotifyOfPropertyChange(() => SettingMD);
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
