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

        public void UpdateFilter(string methodName, object button)
        {
            var type = GetType();
            var method = type.GetMethod(methodName);

            var uIElement = button as UIElement;
            var value = uIElement.Uid;

            var parameter = new object[1] { value };
            method.Invoke(this, parameter);
            FilterChanged();
        }

        // Set IsFilterChanged true
        public void FilterChanged() => Selector.IsFilterChanged = true;

        // Update Filter.ButtonTunes
        public void AddButtonTune(string value) => Filter.ButtonTunes.Add(value);
        public void RemoveButtonTune(string value) => Filter.ButtonTunes.Remove(value);

        // Update Filter.Difficulties
        public void AddDifficulty(string value) => Filter.Difficulties.Add(value);
        public void RemoveDifficulty(string value) => Filter.Difficulties.Remove(value);

        // Update Filter.Categories
        public void AddCategory(string value) => Filter.Categories.Add(value);
        public void RemoveCategory(string value) => Filter.Categories.Remove(value);

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
