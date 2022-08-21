using DjmaxRandomSelectorV.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Playlist
    {
        #region Fields
        private List<Music> _musicList;
        private List<Music> _recents;
        private bool _isUpdated;
        #endregion

        #region Properties
        public List<Music> MusicList
        {
            get { return _musicList; }
            set { _musicList = value; }
        }
        public List<Music> Recents
        {
            get { return _recents; }
            set { _recents = value; }
        }
        public bool IsUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }
        #endregion

        public void UpdateRecents(int musicCount, int maxCount)
        {
            int recentsCount = _recents.Count;
            if (recentsCount > maxCount)
            {
                _recents.RemoveRange(0, recentsCount - maxCount);
            }
            else if (recentsCount >= musicCount)
            {
                try
                {
                    _recents.RemoveRange(0, recentsCount - musicCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
        }
    }
}
