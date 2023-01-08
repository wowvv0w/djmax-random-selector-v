using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    public interface IFilterHandlerBuilder
    {
        void Reset();
        void Build(FilterOption option);
    }
}
