using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class LocatorService : ILocator
    {
        private IReadOnlyDictionary<int, LocationInfo> _locationMap = null;

        public int InputInterval { get; set; }
        public MusicForm MusicForm { get; set; }
        public InputMethod InputMethod { get; set; }

        public void SetLocationMap(IEnumerable<Track> tracks)
        {
            _locationMap = Locator.MakeLocationMap(tracks);
        }

        public void Locate(Pattern pattern)
        {
            if (InputMethod == InputMethod.NotInput || pattern is null)
            {
                return;
            }
            LocationInfo locationInfo = _locationMap[pattern.TrackId];
            string style = MusicForm == MusicForm.Free ? string.Empty : pattern.Style;
            Locator.Locate(locationInfo, style, InputInterval, InputMethod == InputMethod.WithAutoStart);
        }
    }
}
