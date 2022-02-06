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
            get { return _ownedDlcs.Contains(_P3); }
            set
            {
                if (value) { _ownedDlcs.Add(_P3); }
                else { _ownedDlcs.Remove(_P3); }
                NotifyOfPropertyChange(() => OptionP3);
            }
        }
        public bool OptionTR
        {
            get { return _ownedDlcs.Contains(_TR); }
            set
            {
                if (value) { _ownedDlcs.Add(_TR); }
                else { _ownedDlcs.Remove(_TR); }
                NotifyOfPropertyChange(() => OptionTR);
            }
        }
        public bool OptionCE
        {
            get { return _ownedDlcs.Contains(_CE); }
            set
            {
                if (value) { _ownedDlcs.Add(_CE); }
                else { _ownedDlcs.Remove(_CE); }
                NotifyOfPropertyChange(() => OptionCE);
            }
        }
        public bool OptionBS
        {
            get { return _ownedDlcs.Contains(_BS); }
            set
            {
                if (value) { _ownedDlcs.Add(_BS); }
                else { _ownedDlcs.Remove(_BS); }
                NotifyOfPropertyChange(() => OptionBS);
            }
        }
        public bool OptionVE
        {
            get { return _ownedDlcs.Contains(_VE); }
            set
            {
                if (value) { _ownedDlcs.Add(_VE); }
                else { _ownedDlcs.Remove(_VE); }
                NotifyOfPropertyChange(() => OptionVE);
            }
        }
        public bool OptionVE2
        {
            get { return _ownedDlcs.Contains(_VE2); }
            set
            {
                if (value) { _ownedDlcs.Add(_VE2); }
                else { _ownedDlcs.Remove(_VE2); }
                NotifyOfPropertyChange(() => OptionVE2);
            }
        }
        public bool OptionES
        {
            get { return _ownedDlcs.Contains(_ES); }
            set
            {
                if (value) { _ownedDlcs.Add(_ES); }
                else { _ownedDlcs.Remove(_ES); }
                NotifyOfPropertyChange(() => OptionES);
            }
        }
        public bool OptionT1
        {
            get { return _ownedDlcs.Contains(_T1); }
            set
            {
                if (value) { _ownedDlcs.Add(_T1); }
                else { _ownedDlcs.Remove(_T1); }
                NotifyOfPropertyChange(() => OptionT1);
            }
        }
        public bool OptionT2
        {
            get { return _ownedDlcs.Contains(_T2); }
            set
            {
                if (value) { _ownedDlcs.Add(_T2); }
                else { _ownedDlcs.Remove(_T2); }
                NotifyOfPropertyChange(() => OptionT2);
            }
        }
        public bool OptionT3
        {
            get { return _ownedDlcs.Contains(_T3); }
            set
            {
                if (value) { _ownedDlcs.Add(_T3); }
                else { _ownedDlcs.Remove(_T3); }
                NotifyOfPropertyChange(() => OptionT3);
            }
        }
        public bool OptionCHU
        {
            get { return _ownedDlcs.Contains(_CHU); }
            set
            {
                if (value) { _ownedDlcs.Add(_CHU); }
                else { _ownedDlcs.Remove(_CHU); }
                NotifyOfPropertyChange(() => OptionCHU);
            }
        }
        public bool OptionCY
        {
            get { return _ownedDlcs.Contains(_CY); }
            set
            {
                if (value) { _ownedDlcs.Add(_CY); }
                else { _ownedDlcs.Remove(_CY); }
                NotifyOfPropertyChange(() => OptionCY);
            }
        }
        public bool OptionDM
        {
            get { return _ownedDlcs.Contains(_DM); }
            set
            {
                if (value) { _ownedDlcs.Add(_DM); }
                else { _ownedDlcs.Remove(_DM); }
                NotifyOfPropertyChange(() => OptionDM);
            }
        }
        public bool OptionESTI
        {
            get { return _ownedDlcs.Contains(_ESTI); }
            set
            {
                if (value) { _ownedDlcs.Add(_ESTI); }
                else { _ownedDlcs.Remove(_ESTI); }
                NotifyOfPropertyChange(() => OptionESTI);
            }
        }
        public bool OptionGC
        {
            get { return _ownedDlcs.Contains(_GC); }
            set
            {
                if (value) { _ownedDlcs.Add(_GC); }
                else { _ownedDlcs.Remove(_GC); }
                NotifyOfPropertyChange(() => OptionGC);
            }
        }
        public bool OptionGF
        {
            get { return _ownedDlcs.Contains(_GF); }
            set
            {
                if (value) { _ownedDlcs.Add(_GF); }
                else { _ownedDlcs.Remove(_GF); }
                NotifyOfPropertyChange(() => OptionGF);
            }
        }
        public bool OptionNXN
        {
            get { return _ownedDlcs.Contains(_NXN); }
            set
            {
                if (value) { _ownedDlcs.Add(_NXN); }
                else { _ownedDlcs.Remove(_NXN); }
                NotifyOfPropertyChange(() => OptionNXN);
            }
        }
    }
}
