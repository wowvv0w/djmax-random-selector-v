using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public FilterViewModel FilterViewModel { get; set; }

        public MainViewModel()
        {
            this.FilterViewModel = new FilterViewModel();
        }
    }
}
