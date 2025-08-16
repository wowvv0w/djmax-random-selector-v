using System;
using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class TrackDB : ITrackDB
    {
        private const string AllTrackFilePath = @"DMRSV3_Data\AllTrackList.json";

        private readonly IFileManager _fileManager;

        private string[] _basicCategories;
        private Dmrsv3LinkDisc[] _linkDisc;
        private Dictionary<int, Track> _allTrack;

        public IEnumerable<Track> AllTrack => _allTrack.Values;
        public IEnumerable<Track> Playable => _allTrack.Values.Where(t => t.IsPlayable);
        public IReadOnlyList<Dmrsv3Category> Categories { get; private set; }

        public TrackDB(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public Track Find(int trackId)
        {
            _allTrack.TryGetValue(trackId, out Track result);
            return result;
        }

        public Pattern Find(PatternId patternId)
        {
            return Find(patternId.TrackId)?.Patterns.FirstOrDefault(p => p.Id == patternId, null);
        }

        public void Initialize(Dmrsv3AppData appdata)
        {
            _basicCategories = appdata.BasicCategories;
            _linkDisc = appdata.LinkDisc;
            Categories = new List<Dmrsv3Category>(appdata.Categories);
        }

        public void ImportDB()
        {
            var db = _fileManager.Import<List<VArchiveDBTrack>>(AllTrackFilePath);
            _allTrack = db.Select(x =>
            {
                var info = new MusicInfo()
                {
                    Title = x.Name,
                    Composer = x.Composer,
                    Category = x.DlcCode
                };
                return new Track()
                {
                    Id = x.Title,
                    Info = info,
                    Patterns = x.Patterns
                                .SelectMany(bt => bt.Value, (bt, df) => new Pattern()
                                {
                                    Id = new PatternId(x.Title, bt.Key.AsButtonTunes(), df.Key.AsDifficulty()),
                                    Info = info,
                                    Level = df.Value.Level
                                })
                                .OrderBy(p => p.Id)
                                .ToArray(),
                    IsPlayable = false
                };
            }).ToDictionary(t => t.Id);
        }

        public void SetPlayable(IEnumerable<string> ownedDlcs)
        {
            var categories = ownedDlcs.Concat(_basicCategories);
            var exclusions = _linkDisc.Where(x => !x.IsRequired(ownedDlcs)).Select(x => x.Id);
            _allTrack = _allTrack.Values
                .Select(t => t with { IsPlayable = categories.Contains(t.Category) && !exclusions.Contains(t.Id) })
                .ToDictionary(t => t.Id);
        }
    }
}
