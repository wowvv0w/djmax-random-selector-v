using System;

namespace DjmaxRandomSelectorV.States
{
    public interface IReadOnlyVersionInfoState
    {
        public Version CurrentAppVersion { get; }
        public Version LatestAppVersion { get; }
        public long AllTrackVersion { get; }
        public string AppdataVersion { get; }
    }
}
