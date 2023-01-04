using Caliburn.Micro;
using Dmrsv.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public abstract class FilterBaseViewModel : Screen
    {
        public FilterBaseViewModel()
        {
            DisplayName = "FILTER";
        }
    }
}
