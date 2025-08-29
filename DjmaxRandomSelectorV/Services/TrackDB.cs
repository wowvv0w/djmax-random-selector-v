using System;
using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;
using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class TrackDB : ITrackDB
    {
        private string[] _basicCategories;
        private LinkDiscChecker _linkDiscChecker;
        private Dictionary<int, Track> _allTrack;

        public IEnumerable<Track> AllTrack => _allTrack.Values;
        public IEnumerable<Track> Playable => _allTrack.Values.Where(t => t.IsPlayable);
        public IReadOnlyList<Dmrsv3Category> Categories { get; private set; }

        public Track Find(int trackId)
        {
            _allTrack.TryGetValue(trackId, out Track result);
            return result;
        }

        public Pattern Find(PatternId patternId)
        {
            return Find(patternId.TrackId)?.Patterns.FirstOrDefault(p => p.Id == patternId, null);
        }

        public void Initialize(Dmrsv3Appdata appdata, Dictionary<int, Track> allTrack)
        {
            _basicCategories = appdata.BasicCategories;
            _linkDiscChecker = new LinkDiscChecker(appdata.LinkDisc);
            Categories = new List<Dmrsv3Category>(appdata.Categories);
            _allTrack = allTrack;
        }

        public void SetPlayable(IEnumerable<string> ownedDlcs)
        {
            var categories = ownedDlcs.Concat(_basicCategories).ToHashSet();
            var exclusions = _linkDiscChecker.GetExclusionSet(ownedDlcs);
            bool GetIsPlayable(Track t) => categories.Contains(t.Category) && !exclusions.Contains(t.Id);
            _allTrack = _allTrack.Values
                                 .Select(t => t with { IsPlayable = GetIsPlayable(t) })
                                 .ToDictionary(t => t.Id);
        }

        private class LinkDiscChecker
        {
            private readonly Dmrsv3LinkDisc[] _linkDiscs;
            public LinkDiscChecker(Dmrsv3LinkDisc[] linkDisc)
            {
                _linkDiscs = linkDisc;
            }
            public HashSet<int> GetExclusionSet(IEnumerable<string> ownedDlcs)
            {
                bool IsSatisfied(Dmrsv3LinkDisc disc)
                {
                    return disc.RequiredDlc.Any(required => required.All(dlc => ownedDlcs.Contains(dlc)));
                }
                return _linkDiscs.Where(disc => !IsSatisfied(disc)).Select(disc => disc.Id).ToHashSet();
            }
        }
    }
}
