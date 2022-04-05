using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes.Interfaces
{
    public interface IAddonObservable
    {
        void Subscribe(IAddonObserver observer);
        void Notify();
    }
}
