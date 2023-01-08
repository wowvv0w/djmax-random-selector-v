using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    public record FilterHandlerOption(MusicForm Mode, LevelPreference Level);

    public interface IFilterHandlerBuilder
    {
        void Reset();
        void Build(FilterHandlerOption option);
    }
}
