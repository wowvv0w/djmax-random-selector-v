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
        public Dictionary<string, FilterUpdater> Setting { get; set; }
            = new Dictionary<string, FilterUpdater>()
            {
                ["4B"] = new ButtonTuneUpdater("4B"), 
                ["5B"] = new ButtonTuneUpdater("5B"),
                ["6B"] = new ButtonTuneUpdater("6B"),
                ["8B"] = new ButtonTuneUpdater("8B"),

                ["NM"] = new DifficultyUpdater("NM"),
                ["HD"] = new DifficultyUpdater("HD"),
                ["MX"] = new DifficultyUpdater("MX"),
                ["SC"] = new DifficultyUpdater("SC"),

                ["RP"] = new CategoryUpdater("RP"),
                ["P1"] = new CategoryUpdater("P1"),
                ["P2"] = new CategoryUpdater("P2"),
                ["P3"] = new CategoryUpdater("P3"),
                ["TR"] = new CategoryUpdater("TR"),
                ["CE"] = new CategoryUpdater("CE"),
                ["BS"] = new CategoryUpdater("BS"),
                ["VE"] = new CategoryUpdater("VE"),
                ["VE2"] = new CategoryUpdater("VE2"),
                ["ES"] = new CategoryUpdater("ES"),
                ["T1"] = new CategoryUpdater("T1"),
                ["T2"] = new CategoryUpdater("T2"),
                ["T3"] = new CategoryUpdater("T3"),

                ["GG"] = new CategoryUpdater("GG"),
                ["CHU"] = new CategoryUpdater("CHU"),
                ["CY"] = new CategoryUpdater("CY"),
                ["DM"] = new CategoryUpdater("DM"),
                ["ESTI"] = new CategoryUpdater("ESTI"),
                ["GC"] = new CategoryUpdater("GC"),
                ["GF"] = new CategoryUpdater("GF"),
                ["NXN"] = new CategoryUpdater("NXN")
            };
    }

    public abstract class FilterUpdater : PropertyChangedBase
    {
        private bool _value = false;
        protected string name;
        public bool Value
        {
            get { return _value; }
            set 
            { 
                _value = value; 
                NotifyOfPropertyChange(() => Value); 
                UpdateFilter(name);
                Selector.IsFilterChanged = true;
            }
        }
        protected abstract void UpdateFilter(string name);
        public FilterUpdater(string name)
        {
            this.name = name;
        }
    }

    public class ButtonTuneUpdater : FilterUpdater
    {
        protected override void UpdateFilter(string name)
        {
            if (Value)
            {
                Filter.ButtonTunes.Add(name);
            }
            else
            {
                Filter.ButtonTunes.Remove(name);
            }
        }
        public ButtonTuneUpdater(string name) : base(name)
        {
        }
    }

    public class DifficultyUpdater : FilterUpdater
    {
        protected override void UpdateFilter(string name)
        {
            if (Value)
            {
                Filter.Difficulties.Add(name);
            }
            else
            {
                Filter.Difficulties.Remove(name);
            }
        }
        public DifficultyUpdater(string name) : base(name)
        {
        }
    }
    public class CategoryUpdater : FilterUpdater
    {
        protected override void UpdateFilter(string name)
        {
            if (Value)
            {
                Filter.Categories.Add(name);
            }
            else
            {
                Filter.Categories.Remove(name);
            }
        }
        public CategoryUpdater(string name) : base(name)
        {
        }
    }

    public class LevelUpdater : FilterUpdater
    {
        private int _value;
        public new int Value
        {
            get { return _value; }
            set { _value = value; NotifyOfPropertyChange(() => Value); UpdateFilter(name); }
        }
        protected override void UpdateFilter(string name)
        {
            if (name == "MIN")
            {
                Filter.Levels[0] = Value;
            }
            else if (name == "MAX")
            {
                Filter.Levels[1] = Value;
            }
        }
        public LevelUpdater(string name) : base(name)
        {
        }
    }
}
