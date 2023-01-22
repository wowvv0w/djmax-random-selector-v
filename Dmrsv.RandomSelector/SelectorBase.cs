namespace Dmrsv.RandomSelector
{
    public abstract class SelectorBase : ISelector
    {
        public virtual Music? Select(IEnumerable<Music> musicList)
        {
            if (!musicList.Any() || musicList == null)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(musicList.Count() - 1);
            return musicList.ElementAt(index);
        }
    }

    //[Obsolete]
    //public class Selector2
    //{
    //    private List<Track> _tracks;
    //    private List<Music> _musics;
    //    private List<string> _titles;

    //    private int _maxExclusionCount;
    //    private int _inputInterval;
    //    private List<string> _exclusions;

    //    private bool _isUpdated;
    //    private bool _isRunning;
    //    private bool _canExecuteWithoutGame;

    //    private IFilter? _filter;
    //    private ISifter? _sifter;
    //    private IProvider? _provider;

    //    public Selector2()
    //    {
    //        _tracks = new List<Track>();
    //        _musics = new List<Music>();
    //        _titles = new List<string>();
    //        _exclusions = new List<string>();
    //        _isUpdated = true;
    //        _isRunning = false;
    //        //Handle(new OptionApi().GetSelectorOption());
    //    }

    //    public void LoadConfiguration(Configuration config)
    //    {
    //        UpdateTracks(config.OwnedDlcs);
    //        _exclusions = config.Exclusions;
    //        _isUpdated = true;
    //    }


    //    public Music Start()
    //    {
    //        _isRunning = true;

    //        if (_isUpdated)
    //        {
    //            _musics = _sifter!.Sift(_tracks, _filter!);
    //            _exclusions.Clear();
    //            var titleList = from music in _musics
    //                            select music.Title;
    //            _titles = titleList.Distinct().ToList();
    //            _isUpdated = false;
    //        }
    //        else
    //        {
    //            UpdateExclusions();
    //        }

    //        var filteredList = (from music in _musics
    //                           where !_exclusions.Contains(music.Title)
    //                           select music).ToList();

    //        Music selectedMusic;
    //        if (filteredList.Any())
    //        {
    //            var random = new Random();
    //            int index = random.Next(filteredList.Count - 1);
    //            selectedMusic = filteredList[index];

    //            _provider?.Provide(selectedMusic, _tracks, _inputInterval);
    //            _exclusions.Add(selectedMusic.Title);
    //        }
    //        else
    //        {
    //            throw new Exception("There is no music that meets the filter conditions.");
    //        }

    //        _isRunning = false;
    //        return selectedMusic;
    //    }

    //    private void ChangeSifter(FilterType filterType)
    //    {
    //        _sifter = filterType switch
    //        {
    //            FilterType.Query => new QuerySifter(),
    //            FilterType.Playlist => new PlaylistSifter(),
    //            _ => throw new NotSupportedException(),
    //        };
    //        Handle(new OptionApi().GetFilterOption());
    //    }
    //    private void ChangeProvider(Mode mode, Aider aider)
    //    {
    //        bool isFreestyle = mode == Mode.Freestyle;
    //        _provider = aider switch
    //        {
    //            Aider.Off => new Locator(false),
    //            Aider.AutoStart => new Locator(isFreestyle),
    //            Aider.Observe => null,
    //            _ => throw new NotSupportedException(),
    //        };
    //    }

    //    public void Handle(FilterOption message)
    //    {
    //        _maxExclusionCount = message.Except; // selector
    //        _sifter?.ChangeMethod(message); // filterhandlerbuilder
    //        ChangeProvider(message.Mode, message.Aider); // locator
    //        _canExecuteWithoutGame = message.Aider == Aider.Observe;
    //        _isUpdated = true;
    //    }

    //    public void Handle(SelectorOption message)
    //    {
    //        ChangeSifter(message.FilterType); // filterhandler
    //        _inputInterval = message.InputInterval; // locator
    //        UpdateTracks(message.OwnedDlcs); // track
    //        _isUpdated = true;
    //    }
}
