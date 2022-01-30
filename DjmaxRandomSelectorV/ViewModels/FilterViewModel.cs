using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterViewModel : Screen
    {
        public List<Track> TrackList { get; set; }

        public FilterViewModel()
        {
            for(int i = 0; i < 16; i++)
            {
                /// DO NOT use index 0
                LevelIndicators.Add(new LevelIndicator());
            }
            TrackList = Manager.ReadTrackList();
        }
        public int LevelMin
        {
            get { return Filter.Levels[0]; }
            set 
            {
                Filter.Levels[0] = value;
                NotifyOfPropertyChange(() => LevelMin);
            }
        }
        public int LevelMax
        {
            get { return Filter.Levels[1]; }
            set
            {
                Filter.Levels[1] = value;
                NotifyOfPropertyChange(() => LevelMax);
            }
        }


        /// Update Filter.ButtonTunes
        public void AddButtonTune(string value)
        {
            Filter.ButtonTunes.Add(value);
        }
        public void RemoveButtonTune(string value)
        {
            Filter.ButtonTunes.Remove(value);
        }
       
        /// Update Filter.Difficulties
        public void AddDifficulty(string value)
        {
            Filter.Difficulties.Add(value);
        }
        public void RemoveDifficulty(string value)
        {
            Filter.Difficulties.Remove(value);
        }

        /// Update Filter.Categories
        public void AddCategory(string value)
        {
            Filter.Categories.Add(value);
        }
        public void RemoveCategory(string value)
        {
            Filter.Categories.Remove(value);
        }

        /// Update Filter.Level
        public void IncreaseLevelMin()
        {
            if(LevelMin < 15 && LevelMin < LevelMax)
            {
                LevelMin++;
                UpdateLevelIndicators();
            }
        }
        public void DecreaseLevelMin()
        {
            if (LevelMin > 1)
            {
                LevelMin--;
                UpdateLevelIndicators();
            }
        }
        public void IncreaseLevelMax()
        {
            if (LevelMax < 15)
            {
                LevelMax++;
                UpdateLevelIndicators();
            }
        }
        public void DecreaseLevelMax()
        {
            if (LevelMax > 1 && LevelMax > LevelMin)
            {
                LevelMax--;
                UpdateLevelIndicators();
            }
        }

        /// Update LevelIndicators
        public void UpdateLevelIndicators()
        {
            for(int i = 1; i < LevelMin; i++)
            {
                LevelIndicators[i].Value = false;
            }
            for (int i = LevelMin; i <= LevelMax; i++)
            {
                LevelIndicators[i].Value = true;
            }
            for (int i = LevelMax+1; i <= 15; i++)
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
        public ObservableCollection<LevelIndicator> LevelIndicators { get; set; }
            = new ObservableCollection<LevelIndicator>();

    }
}
