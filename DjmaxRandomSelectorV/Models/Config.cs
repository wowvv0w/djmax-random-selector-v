using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DjmaxRandomSelectorV.Models
{
    public class Config : IAddonObservable
    {
        private readonly List<IAddonObserver> observers;

        private int _except;
        public int Except
        { 
            get { return _except; }
            set
            {
                _except = value;
                Notify();
            }
        }
        private Mode _mode;
        public Mode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                Notify();
            }
        }
        private Aider _aider;
        public Aider Aider
        {
            get { return _aider; }
            set
            {
                _aider = value;
                Notify();
            }
        }
        private Level _level;
        public Level Level
        {
            get { return _level; }
            set
            {
                _level = value;
                Notify();
            }
        }
        public bool IsPlaylist { get; set; }
        public int InputDelay { get; set; }
        public bool SavesRecents { get; set; }
        public List<string> OwnedDlcs { get; set; }
        public double[] Position { get; set; }
        public List<string> Favorite { get; set; }
        public int AllTrackVersion { get; set; }

        public Config()
        {
            observers = new List<IAddonObserver>();
        }

        #region IAddonObservable Methods
        public void Subscribe(IAddonObserver observer) => observers.Add(observer);
        public void Notify()
        {
            foreach (var observer in observers)
                observer.Update(this);
        }
        #endregion

    }
}
