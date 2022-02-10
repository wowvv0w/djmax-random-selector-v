using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.Properties;
using DjmaxRandomSelectorV.Models;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class OptionViewModel : Screen
    {
        public OptionViewModel()
        {
            _inputDelay = Settings.Default.inputDelay;
            _ownedDlcs = new List<string>();
            foreach (var dlc in Settings.Default.ownedDlcs)
            {
                _ownedDlcs.Add(dlc);
            }
            UpdateInputDelayText();
        }

        private int _inputDelay;
        private List<string> _ownedDlcs;

        public void ApplyOption()
        {
            Settings.Default.inputDelay = _inputDelay;
            Settings.Default.ownedDlcs = new StringCollection();
            foreach (var dlc in _ownedDlcs)
            {
                Settings.Default.ownedDlcs.Add(dlc);
            }
            Settings.Default.Save();
            Manager.UpdateTrackList();
            Selector.IsFilterChanged = true;
            TryCloseAsync();
        }
        public void CancelOption()
        {
            TryCloseAsync();
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

        public bool OptionP3
        {
            get { return CheckOwnedDlcs(_P3); }
            set
            {
                UpdateOwnedDlcs(value, _P3);
                NotifyOfPropertyChange(() => OptionP3);
            }
        }
        public bool OptionTR
        {
            get { return CheckOwnedDlcs(_TR); }
            set
            {
                UpdateOwnedDlcs(value, _TR);
                NotifyOfPropertyChange(() => OptionTR);
            }
        }
        public bool OptionCE
        {
            get { return CheckOwnedDlcs(_CE); }
            set
            {
                UpdateOwnedDlcs(value, _CE);
                NotifyOfPropertyChange(() => OptionCE);
            }
        }
        public bool OptionBS
        {
            get { return CheckOwnedDlcs(_BS); }
            set
            {
                UpdateOwnedDlcs(value, _BS);
                NotifyOfPropertyChange(() => OptionBS);
            }
        }
        public bool OptionVE
        {
            get { return CheckOwnedDlcs(_VE); }
            set
            {
                UpdateOwnedDlcs(value, _VE);
                NotifyOfPropertyChange(() => OptionVE);
            }
        }
        public bool OptionVE2
        {
            get { return CheckOwnedDlcs(_VE2); }
            set
            {
                UpdateOwnedDlcs(value, _VE2);
                NotifyOfPropertyChange(() => OptionVE2);
            }
        }
        public bool OptionES
        {
            get { return CheckOwnedDlcs(_ES); }
            set
            {
                UpdateOwnedDlcs(value, _ES);
                NotifyOfPropertyChange(() => OptionES);
            }
        }
        public bool OptionT1
        {
            get { return CheckOwnedDlcs(_T1); }
            set
            {
                UpdateOwnedDlcs(value, _T1);
                NotifyOfPropertyChange(() => OptionT1);
            }
        }
        public bool OptionT2
        {
            get { return CheckOwnedDlcs(_T2); }
            set
            {
                UpdateOwnedDlcs(value, _T2);
                NotifyOfPropertyChange(() => OptionT2);
            }
        }
        public bool OptionT3
        {
            get { return CheckOwnedDlcs(_T3); }
            set
            {
                UpdateOwnedDlcs(value, _T3);
                NotifyOfPropertyChange(() => OptionT3);
            }
        }
        public bool OptionCHU
        {
            get { return CheckOwnedDlcs(_CHU); }
            set
            {
                UpdateOwnedDlcs(value, _CHU);
                NotifyOfPropertyChange(() => OptionCHU);
            }
        }
        public bool OptionCY
        {
            get { return CheckOwnedDlcs(_CY); }
            set
            {
                UpdateOwnedDlcs(value, _CY);
                NotifyOfPropertyChange(() => OptionCY);
            }
        }
        public bool OptionDM
        {
            get { return CheckOwnedDlcs(_DM); }
            set
            {
                UpdateOwnedDlcs(value, _DM);
                NotifyOfPropertyChange(() => OptionDM);
            }
        }
        public bool OptionESTI
        {
            get { return CheckOwnedDlcs(_ESTI); }
            set
            {
                UpdateOwnedDlcs(value, _ESTI);
                NotifyOfPropertyChange(() => OptionESTI);
            }
        }
        public bool OptionGC
        {
            get { return CheckOwnedDlcs(_GC); }
            set
            {
                UpdateOwnedDlcs(value, _GC);
                NotifyOfPropertyChange(() => OptionGC);
            }
        }
        public bool OptionGF
        {
            get { return CheckOwnedDlcs(_GF); }
            set
            {
                UpdateOwnedDlcs(value, _GF);
                NotifyOfPropertyChange(() => OptionGF);
            }
        }
        public bool OptionNXN
        {
            get { return CheckOwnedDlcs(_NXN); }
            set
            {
                UpdateOwnedDlcs(value, _NXN);
                NotifyOfPropertyChange(() => OptionNXN);
            }
        }
    }
}
