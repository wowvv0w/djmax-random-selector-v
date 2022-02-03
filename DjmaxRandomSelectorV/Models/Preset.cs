using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Preset
    {
        public Dictionary<string, IsCheckedUpdater> Setting { get; set; }
            = new Dictionary<string, IsCheckedUpdater>()
            {
                ["4B"] = new IsCheckedUpdater(),
                ["5B"] = new IsCheckedUpdater(),
                ["6B"] = new IsCheckedUpdater(),
                ["8B"] = new IsCheckedUpdater(),

                ["NM"] = new IsCheckedUpdater(),
                ["HD"] = new IsCheckedUpdater(),
                ["MX"] = new IsCheckedUpdater(),
                ["SC"] = new IsCheckedUpdater(),

                ["RP"] = new IsCheckedUpdater(),
                ["P1"] = new IsCheckedUpdater(),
                ["P2"] = new IsCheckedUpdater(),
                ["P3"] = new IsCheckedUpdater(),
                ["TR"] = new IsCheckedUpdater(),
                ["CE"] = new IsCheckedUpdater(),
                ["BS"] = new IsCheckedUpdater(),
                ["VE"] = new IsCheckedUpdater(),
                ["VE2"] = new IsCheckedUpdater(),
                ["ES"] = new IsCheckedUpdater(),
                ["T1"] = new IsCheckedUpdater(),
                ["T2"] = new IsCheckedUpdater(),
                ["T3"] = new IsCheckedUpdater(),

                ["GG"] = new IsCheckedUpdater(),
                ["CHU"] = new IsCheckedUpdater(),
                ["CY"] = new IsCheckedUpdater(),
                ["DM"] = new IsCheckedUpdater(),
                ["ESTI"] = new IsCheckedUpdater(),
                ["GC"] = new IsCheckedUpdater(),
                ["GF"] = new IsCheckedUpdater(),
                ["NXN"] = new IsCheckedUpdater()
            };
    }

    public class IsCheckedUpdater : PropertyChangedBase
    {
        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
        }
    }

    public class ValueUpdater : PropertyChangedBase
    {
        private int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; NotifyOfPropertyChange(() => Value); }
        }
    }
}
