using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistViewModel : Screen
    {
        public BindableCollection<Music> PlaylistItems { get; set; }

        public PlaylistViewModel()
        {
            PlaylistItems = new BindableCollection<Music>()
            {
                new Music()
                {
                    Title = "ALiCE",
                    Style = "5BMX",
                    Level = "13"
                }
            };
        }
    }
}
