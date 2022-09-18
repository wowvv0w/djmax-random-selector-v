using Dmrsv.Data.Context.Static;

namespace Dmrsv.Data.Context.Core
{
    public class DmrsvDataContext
    {
        protected DmrsvDataManager Data;

        public DmrsvDataContext()
        {
            Data = DmrsvDataManager.Instance;
        }
    }
}