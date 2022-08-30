using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistViewModel : Screen
    {
        private readonly Playlist _playlist;
        public List<Music> PlaylistItems
        {
            get { return _playlist.MusicList; }
            set
            {
                _playlist.MusicList = value;
                NotifyOfPropertyChange(() => PlaylistItems);
            }
        }

        public PlaylistViewModel(Playlist playlist)
        {
            _playlist = playlist;
        }
    }
}
