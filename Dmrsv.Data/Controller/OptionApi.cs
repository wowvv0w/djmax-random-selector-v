using Dmrsv.Data.Context.Core;
using Dmrsv.Data.Context.Schema;

namespace Dmrsv.Data.Controller
{
    public class OptionApi : DmrsvDataContext
    {
        public FilterOption GetFilterOption()
        {
            var option = Data.FilterOption;
            return new FilterOption()
            {
                Except = option.Except,
                Mode = option.Mode,
                Aider = option.Aider,
                Level = option.Level,
            };
        }

        public void SetFilterOption(FilterOption option)
        {
            Data.FilterOption = new FilterOption()
            {
                Except = option.Except,
                Mode = option.Mode,
                Aider = option.Aider,
                Level = option.Level,
            };
        }

        public SelectorOption GetSelectorOption()
        {
            var option = Data.SelectorOption;
            return new SelectorOption()
            {
                FilterType = option.FilterType,
                InputInterval = option.InputInterval,
                SavesExclusion = option.SavesExclusion,
                OwnedDlcs = new List<string>(option.OwnedDlcs),
            };
        }

        public void SetSelectorOption(SelectorOption option)
        {
            Data.SelectorOption = new SelectorOption()
            {
                FilterType = option.FilterType,
                InputInterval = option.InputInterval,
                SavesExclusion = option.SavesExclusion,
                OwnedDlcs = new List<string>(option.OwnedDlcs),
            };
        }

        public AppOption GetAppOption()
        {
            var option = Data.AppOption;
            return new AppOption()
            {
                Position = (double[])option.Position.Clone(),
                AllTrackVersion = option.AllTrackVersion,
            };
        }

        public void SetAppOption(AppOption option)
        {
            Data.AppOption = new AppOption()
            {
                Position = (double[])option.Position.Clone(),
                AllTrackVersion = option.AllTrackVersion,
            };
        }

        public void SaveConfig()
        {
            Data.Export(Data.Config, "Data/Config.json");
        }
    }
}
