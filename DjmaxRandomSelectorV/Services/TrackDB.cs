using System;
using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;
using DjmaxRandomSelectorV.RandomSelector;
using DjmaxRandomSelectorV.States;

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

        public void SetUserTags(ISettingState setting)
        {
            var categories = setting.OwnedDlcs.Concat(_basicCategories).ToHashSet();
            var exclusions = _linkDiscChecker.GetExclusionSet(setting.OwnedDlcs);
            TrackUserTags GetUserTags(Track t)
            {
                TrackUserTags userTags = TrackUserTags.None;
                if (categories.Contains(t.Category) && !exclusions.Contains(t.Id))
                {
                    userTags |= TrackUserTags.Playable;
                }
                if (setting.Favorite.Contains(t.Id))
                {
                    userTags |= TrackUserTags.Favorite;
                }
                if (setting.Blacklist.Contains(t.Id))
                {
                    userTags |= TrackUserTags.Blacklist;
                }
                return userTags;
            }

            _allTrack = _allTrack.Values
                 .Select(t =>
                 {
                     var newInfo = t.Info with { UserTags = GetUserTags(t) };
                     return t with
                     {
                         Info = newInfo,
                         Patterns = t.Patterns.Select(p => p with { Info = newInfo }).ToArray()
                     };
                 })
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
