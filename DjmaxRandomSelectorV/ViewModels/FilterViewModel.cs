using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DjmaxRandomSelectorV.Properties;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterViewModel : Screen
    {
        public FilterViewModel()
        {
            for(int i = 0; i < 16; i++)
            {
                // DO NOT use index 0
                LevelIndicators.Add(new LevelIndicator());
            }
            Manager.ReadAllTrackList();
            Manager.UpdateTrackList();
        }

        private const string _4B = "4B";
        private const string _5B = "5B";
        private const string _6B = "6B";
        private const string _8B = "8B";
        private const string _NM = "NM";
        private const string _HD = "HD";
        private const string _MX = "MX";
        private const string _SC = "SC";
        private const string _RP = "RP";
        private const string _P1 = "P1";
        private const string _P2 = "P2";
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
        private const string _GG = "GG";
        private const string _CHU = "CHU";
        private const string _CY = "CY";
        private const string _DM = "DM";
        private const string _ESTI = "ESTI";
        private const string _GC = "GC";
        private const string _GF = "GF";
        private const string _NXN = "NXN";

        public bool ButtonTune4B
        {
            get { return Filter.ButtonTunes.Contains(_4B); }
            set
            {
                if (value) { Filter.ButtonTunes.Add(_4B); }
                else { Filter.ButtonTunes.Remove(_4B); }
                NotifyOfPropertyChange(() => ButtonTune4B);
                Selector.IsFilterChanged = true;
            }
        }
        public bool ButtonTune5B
        {
            get { return Filter.ButtonTunes.Contains(_5B); }
            set
            {
                if (value) { Filter.ButtonTunes.Add(_5B); }
                else { Filter.ButtonTunes.Remove(_5B); }
                NotifyOfPropertyChange(() => ButtonTune5B);
                Selector.IsFilterChanged = true;
            }
        }
        public bool ButtonTune6B
        {
            get { return Filter.ButtonTunes.Contains(_6B); }
            set
            {
                if (value) { Filter.ButtonTunes.Add(_6B); }
                else { Filter.ButtonTunes.Remove(_6B); }
                NotifyOfPropertyChange(() => ButtonTune6B);
                Selector.IsFilterChanged = true;
            }
        }
        public bool ButtonTune8B
        {
            get { return Filter.ButtonTunes.Contains(_8B); }
            set
            {
                if (value) { Filter.ButtonTunes.Add(_8B); }
                else { Filter.ButtonTunes.Remove(_8B); }
                NotifyOfPropertyChange(() => ButtonTune8B);
                Selector.IsFilterChanged = true;
            }
        }
        public bool DifficultyNM
        {
            get { return Filter.Difficulties.Contains(_NM); }
            set
            {
                if (value) { Filter.Difficulties.Add(_NM); }
                else { Filter.Difficulties.Remove(_NM); }
                NotifyOfPropertyChange(() => DifficultyNM);
                Selector.IsFilterChanged = true;
            }
        }
        public bool DifficultyHD
        {
            get { return Filter.Difficulties.Contains(_HD); }
            set
            {
                if (value) { Filter.Difficulties.Add(_HD); }
                else { Filter.Difficulties.Remove(_HD); }
                NotifyOfPropertyChange(() => DifficultyHD);
                Selector.IsFilterChanged = true;
            }
        }
        public bool DifficultyMX
        {
            get { return Filter.Difficulties.Contains(_MX); }
            set
            {
                if (value) { Filter.Difficulties.Add(_MX); }
                else { Filter.Difficulties.Remove(_MX); }
                NotifyOfPropertyChange(() => DifficultyMX);
                Selector.IsFilterChanged = true;
            }
        }
        public bool DifficultySC
        {
            get { return Filter.Difficulties.Contains(_SC); }
            set
            {
                if (value) { Filter.Difficulties.Add(_SC); }
                else { Filter.Difficulties.Remove(_SC); }
                NotifyOfPropertyChange(() => DifficultySC);
                Selector.IsFilterChanged = true;
            }
        }
        public int LevelMin
        {
            get { return Filter.Levels[0]; }
            set
            {
                Filter.Levels[0] = value;
                NotifyOfPropertyChange(() => LevelMin);
                UpdateLevelIndicators();
            }
        }
        public int LevelMax
        {
            get { return Filter.Levels[1]; }
            set
            {
                Filter.Levels[1] = value;
                NotifyOfPropertyChange(() => LevelMax);
                UpdateLevelIndicators();
            }
        }
        public bool CategoryRP
        {
            get { return Filter.Categories.Contains(_RP); }
            set
            {
                if (value) { Filter.Categories.Add(_RP); }
                else { Filter.Categories.Remove(_RP); }
                NotifyOfPropertyChange(() => CategoryRP);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryP1
        {
            get { return Filter.Categories.Contains(_P1); }
            set
            {
                if (value) { Filter.Categories.Add(_P1); }
                else { Filter.Categories.Remove(_P1); }
                NotifyOfPropertyChange(() => CategoryP1);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryP2
        {
            get { return Filter.Categories.Contains(_P2); }
            set
            {
                if (value) { Filter.Categories.Add(_P2); }
                else { Filter.Categories.Remove(_P2); }
                NotifyOfPropertyChange(() => CategoryP2);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryP3
        {
            get { return Filter.Categories.Contains(_P3); }
            set
            {
                if (value) { Filter.Categories.Add(_P3); }
                else { Filter.Categories.Remove(_P3); }
                NotifyOfPropertyChange(() => CategoryP3);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryTR
        {
            get { return Filter.Categories.Contains(_TR); }
            set
            {
                if (value) { Filter.Categories.Add(_TR); }
                else { Filter.Categories.Remove(_TR); }
                NotifyOfPropertyChange(() => CategoryTR);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryCE
        {
            get { return Filter.Categories.Contains(_CE); }
            set
            {
                if (value) { Filter.Categories.Add(_CE); }
                else { Filter.Categories.Remove(_CE); }
                NotifyOfPropertyChange(() => CategoryCE);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryBS
        {
            get { return Filter.Categories.Contains(_BS); }
            set
            {
                if (value) { Filter.Categories.Add(_BS); }
                else { Filter.Categories.Remove(_BS); }
                NotifyOfPropertyChange(() => CategoryBS);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryVE
        {
            get { return Filter.Categories.Contains(_VE); }
            set
            {
                if (value) { Filter.Categories.Add(_VE); }
                else { Filter.Categories.Remove(_VE); }
                NotifyOfPropertyChange(() => CategoryVE);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryVE2
        {
            get { return Filter.Categories.Contains(_VE2); }
            set
            {
                if (value) { Filter.Categories.Add(_VE2); }
                else { Filter.Categories.Remove(_VE2); }
                NotifyOfPropertyChange(() => CategoryVE2);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryES
        {
            get { return Filter.Categories.Contains(_ES); }
            set
            {
                if (value) { Filter.Categories.Add(_ES); }
                else { Filter.Categories.Remove(_ES); }
                NotifyOfPropertyChange(() => CategoryES);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryT1
        {
            get { return Filter.Categories.Contains(_T1); }
            set
            {
                if (value) { Filter.Categories.Add(_T1); }
                else { Filter.Categories.Remove(_T1); }
                NotifyOfPropertyChange(() => CategoryT1);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryT2
        {
            get { return Filter.Categories.Contains(_T2); }
            set
            {
                if (value) { Filter.Categories.Add(_T2); }
                else { Filter.Categories.Remove(_T2); }
                NotifyOfPropertyChange(() => CategoryT2);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryT3
        {
            get { return Filter.Categories.Contains(_T3); }
            set
            {
                if (value) { Filter.Categories.Add(_T3); }
                else { Filter.Categories.Remove(_T3); }
                NotifyOfPropertyChange(() => CategoryT3);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryGG
        {
            get { return Filter.Categories.Contains(_GG); }
            set
            {
                if (value) { Filter.Categories.Add(_GG); }
                else { Filter.Categories.Remove(_GG); }
                NotifyOfPropertyChange(() => CategoryGG);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryCHU
        {
            get { return Filter.Categories.Contains(_CHU); }
            set
            {
                if (value) { Filter.Categories.Add(_CHU); }
                else { Filter.Categories.Remove(_CHU); }
                NotifyOfPropertyChange(() => CategoryCHU);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryCY
        {
            get { return Filter.Categories.Contains(_CY); }
            set
            {
                if (value) { Filter.Categories.Add(_CY); }
                else { Filter.Categories.Remove(_CY); }
                NotifyOfPropertyChange(() => CategoryCY);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryDM
        {
            get { return Filter.Categories.Contains(_DM); }
            set
            {
                if (value) { Filter.Categories.Add(_DM); }
                else { Filter.Categories.Remove(_DM); }
                NotifyOfPropertyChange(() => CategoryDM);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryESTI
        {
            get { return Filter.Categories.Contains(_ESTI); }
            set
            {
                if (value) { Filter.Categories.Add(_ESTI); }
                else { Filter.Categories.Remove(_ESTI); }
                NotifyOfPropertyChange(() => CategoryESTI);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryGC
        {
            get { return Filter.Categories.Contains(_GC); }
            set
            {
                if (value) { Filter.Categories.Add(_GC); }
                else { Filter.Categories.Remove(_GC); }
                NotifyOfPropertyChange(() => CategoryGC);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryGF
        {
            get { return Filter.Categories.Contains(_GF); }
            set
            {
                if (value) { Filter.Categories.Add(_GF); }
                else { Filter.Categories.Remove(_GF); }
                NotifyOfPropertyChange(() => CategoryGF);
                Selector.IsFilterChanged = true;
            }
        }
        public bool CategoryNXN
        {
            get { return Filter.Categories.Contains(_NXN); }
            set
            {
                if (value) { Filter.Categories.Add(_NXN); }
                else { Filter.Categories.Remove(_NXN); }
                NotifyOfPropertyChange(() => CategoryNXN);
                Selector.IsFilterChanged = true;
            }
        }

        // Update Filter.Level
        public void IncreaseLevelMin()
        {
            if (LevelMin < 15 && LevelMin < LevelMax)
            {
                LevelMin++;
            }
        }
        public void DecreaseLevelMin()
        {
            if (LevelMin > 1)
            {
                LevelMin--;
            }
        }
        public void IncreaseLevelMax()
        {
            if (LevelMax < 15)
            {
                LevelMax++;
            }
        }
        public void DecreaseLevelMax()
        {
            if (LevelMax > 1 && LevelMax > LevelMin)
            {
                LevelMax--;
            }
        }

        // Update LevelIndicators
        public void UpdateLevelIndicators()
        {
            for (int i = 1; i < LevelMin; i++)
            {
                LevelIndicators[i].Value = false;
            }
            for (int i = LevelMin; i <= LevelMax; i++)
            {
                LevelIndicators[i].Value = true;
            }
            for (int i = LevelMax + 1; i <= 15; i++)
            {
                LevelIndicators[i].Value = false;
            }
        }
        public class LevelIndicator : PropertyChangedBase
        {
            private bool _value;
            public bool Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
            public LevelIndicator()
            {
                _value = true;
            }
        }
        public List<LevelIndicator> LevelIndicators { get; set; }
            = new List<LevelIndicator>();
    }
}
