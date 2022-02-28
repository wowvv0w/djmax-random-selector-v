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


namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private Setting _setting;
        private int _inputDelay;
        private List<string> _ownedDlcs;

        private DockPanel _dockPanel;
        private int dlcCount;

        public SettingViewModel(Setting setting, DockPanel dockPanel)
        {
            _setting = setting;
            _inputDelay = setting.InputDelay;
            _ownedDlcs = setting.OwnedDlcs;
            UpdateInputDelayText();

            _dockPanel = dockPanel;
        }

        public void Detect()
        {
            SettingBS = SettingCE = SettingCHU = SettingCY = SettingDM = SettingES = SettingESTI= SettingGC = SettingGF
                = SettingNXN = SettingP3 = SettingT1 = SettingT2 = SettingT3 = SettingTR = SettingVE = SettingVE2 = false;

            string steamPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
            
            DirectoryInfo libraryPath = new DirectoryInfo(steamPath + "\\appcache\\librarycache");

            dlcCount = 0;
            foreach (FileInfo file in libraryPath.GetFiles())
            {
                if (file.Extension.ToLower().CompareTo(".jpg") == 0)
                {
                    if (FindDLCs(file.Name.Substring(0, file.Name.Length - 11)) == true) dlcCount++;
                }
            }

            MessageBox.Show(dlcCount + " DLCs detected.", "Notice");
        }

        public void Apply()
        {
            _setting.InputDelay = _inputDelay;
            _setting.OwnedDlcs = _ownedDlcs;

            Manager.SaveSetting(_setting);
            Manager.UpdateTrackList(_ownedDlcs);
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
        }

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
        public bool FindDLCs(string name)
        {
            bool hasDLC = true;
            switch (name)
            {
                case "1568680": SettingP3 = true; break;
                case "1271670": SettingTR = true; break;
                case "1300210": SettingCE = true; break;
                case "1300211": SettingBS = true; break;
                case "1080550": SettingVE = true; break;
                case "1843020": SettingVE2 = true; break;
                case "1238760": SettingES = true; break;
                case "1386610": SettingT1 = true; break;
                case "1386611": SettingT2 = true; break;
                case "1386612": SettingT3 = true; break;
                case "1472190": SettingCHU = true; break;
                case "1356221": SettingCY = true; break;
                case "1356220": SettingDM = true; break;
                case "1664550": SettingESTI = true; break;
                case "1271671": SettingGC = true; break;
                case "1472191": SettingGF = true; break;
                case "1782170": SettingNXN = true; break;
                default: hasDLC = false; break;
            }

            return hasDLC;
        }
    }
}
