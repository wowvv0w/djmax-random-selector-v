using System;

namespace DjmaxRandomSelectorV.States
{
    public interface IVersionInfoState
    {
        public Version CurrentAppVersion { get; set; }
        public Version LatestAppVersion { get; set; }
        public long AllTrackVersion { get; set; }
        public string AppdataVersion { get; set; }
    }
}
