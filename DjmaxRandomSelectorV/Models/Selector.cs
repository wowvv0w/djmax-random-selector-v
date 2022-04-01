using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DjmaxRandomSelectorV.Models
{
    public class Selector
    {
        #region Fields
        private List<Track> _allTrackList;
        private List<Track> _trackList;
        private List<Music> _musicList;
        private int _musicCount;
        #endregion

        #region Properties
        public static bool IsFilterChanged { get; set; } = true;
        public bool IsRunning { get; set; }
        #endregion

        #region Constants
        private const string AllTrackListPath = "Data/AllTrackList.csv";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        #endregion

        public Selector()
        {
            IsRunning = false;
        }

        #region Manage Track List
        public List<string> GetTitleList() => _allTrackList.ConvertAll(x => x.Title).Distinct().ToList();
        public void DownloadAllTrackList()
        {
            string data;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                data = client.DownloadString(AllTrackListUrl);
            }

            using (var writer = new StreamWriter(AllTrackListPath))
            {
                writer.Write(data);
            }
        }
        public void ReadAllTrackList()
        {
            using (var reader = new StreamReader(AllTrackListPath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<TrackMap>();
                var records = csv.GetRecords<Track>().ToList();
                _allTrackList = records;
            }
        }
        public void UpdateTrackList(List<string> ownedDlcs)
        {
            var basicCategories = new List<string>() { "RP", "P1", "P2", "GG" };
            var titleFilter = CreateTitleFilter(ownedDlcs);
            var trackList = from track in _allTrackList
                            where (ownedDlcs.Contains(track.Category) || basicCategories.Contains(track.Category))
                            && !titleFilter.Contains(track.Title)
                            select track;

            _trackList = trackList.ToList();
        }
        private List<string> CreateTitleFilter(List<string> ownedDlcs)
        {
            var list = new List<string>();

            if (!ownedDlcs.Contains("P3"))
            {
                list.Add("glory day (Mintorment Remix)");
                list.Add("glory day -JHS Remix-");
            }
            if (!ownedDlcs.Contains("TR"))
                list.Add("Nevermind");
            if (!ownedDlcs.Contains("CE"))
                list.Add("Rising The Sonic");
            if (!ownedDlcs.Contains("BS"))
                list.Add("ANALYS");
            if (!ownedDlcs.Contains("T1"))
                list.Add("Do you want it");
            if (!ownedDlcs.Contains("T2"))
                list.Add("End of Mythology");
            if (!ownedDlcs.Contains("T3"))
                list.Add("ALiCE");
            if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Here in the Moment ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Airwave ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                list.Add("SON OF SUN ~Extended Mix~");
            if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                list.Add("너로피어오라 ~Original Ver.~");

            return list;
        }
        #endregion

        #region Select Music
        public void SiftOut(Filter filter, List<string> favorite, Mode mode, Level level)
        {
            List<string> styles = new List<string>();
            foreach(string button in filter.ButtonTunes)
            {
                foreach(string difficulty in filter.Difficulties)
                {
                    styles.Add($"{button}{difficulty}");
                }
            }

            IEnumerable<Music> musicList;
            if (mode == Mode.Freestyle)
            {
                if (level == Level.Off)
                {
                    musicList =
                        from track in _trackList
                        where filter.Categories.Contains(track.Category) || favorite.Contains(track.Title)
                        from pattern in track.Patterns
                        where styles.Contains(pattern.Key)
                        && pattern.Value >= filter.Levels[0]
                        && pattern.Value <= filter.Levels[1]
                        select new Music
                        {
                            Title = track.Title,
                            Style = pattern.Key,
                            Level = pattern.Value.ToString()
                        };
                }
                else
                {
                    bool getFirst = level == Level.Beginner;
                    musicList =
                        from track in _trackList
                        where filter.Categories.Contains(track.Category) || favorite.Contains(track.Title)
                        from bt in filter.ButtonTunes
                        let _styles = styles.FindAll(x => x.Contains(bt))
                        let _patterns = from pattern in track.Patterns
                                        where _styles.Contains(pattern.Key)
                                        && pattern.Value >= filter.Levels[0]
                                        && pattern.Value <= filter.Levels[1]
                                        select pattern
                        where _patterns.Count() > 0
                        let _pattern = getFirst ? _patterns.First() : _patterns.Last()
                        select new Music
                        {
                            Title = track.Title,
                            Style = _pattern.Key,
                            Level = _pattern.Value.ToString()
                        };
                }
            }
            else
            {
                musicList = 
                    from track in _trackList
                    where (filter.Categories.Contains(track.Category) || favorite.Contains(track.Title))
                    && track.Patterns.Any(x => styles.Contains(x.Key)
                                               && x.Value >= filter.Levels[0]
                                               && x.Value <= filter.Levels[1])
                    select new Music
                    {
                        Title = track.Title,
                        Style = "FREE",
                        Level = "-"
                    };
            }

            _musicList = musicList.ToList();

            var titleList = from music in _musicList
                            select music.Title;
            _musicCount = titleList.Distinct().Count();
        }

        public List<string> CheckRecents(List<string> recents, int count)
        {
            int recentsCount = recents.Count;

            if (recentsCount >= _musicCount)
            {
                try
                {
                    recents.RemoveRange(0, recentsCount - _musicCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
            else if (recentsCount > count)
            {
                recents.RemoveRange(0, recentsCount - count);
            }

            return recents;
        }

        public Music Pick(List<string> recents)
        {
            var musicList = (from music in _musicList
                             where !recents.Contains(music.Title)
                             select music).ToList();

            var random = new Random();
            var index = random.Next(musicList.Count - 1);
            var selectedMusic = musicList[index];

            return selectedMusic;
        }

        public InputCommand Find(Music selectedMusic) 
        {
            // Check if title starts with alphabet or not
            char initial = selectedMusic.Title[0];
            bool isAlphabet = Regex.IsMatch(initial.ToString(), "[a-z]", RegexOptions.IgnoreCase);

            // Create List of Tracks that have same initial with selected music
            List<Track> sameInitialList;
            if (isAlphabet)
            {
                var list = from track in _trackList
                           let t = track.Title.Substring(0, 1)
                           where String.Compare(t, initial.ToString(), true) == 0
                           select track;
                sameInitialList = list.ToList();
            }
            else
            {
                var list = from track in _trackList
                           let t = track.Title.Substring(0, 1)
                           where Regex.IsMatch(t, "[a-z]", RegexOptions.IgnoreCase) == false
                           select track;
                sameInitialList = list.ToList();
            }

            // Find which key should be pressed
            int whereIsIt = sameInitialList.FindIndex(x => x.Title == selectedMusic.Title);
            int count = sameInitialList.Count;
            bool isForward = whereIsIt <= Math.Ceiling((double)count / 2) || "wxyzWXYZ".Contains(initial);

            char inputInitial;
            int inputVertical;
            if (isForward)
            {
                if (isAlphabet)
                {
                    inputInitial = initial;
                }
                else
                {
                    inputInitial = 'a';
                }
                inputVertical = whereIsIt;
            }
            else
            {
                if (isAlphabet)
                {
                    inputInitial = (char)(initial + 1);
                }
                else
                {
                    inputInitial = 'a';
                }
                inputVertical = count - whereIsIt;
            }

            int inputRight;
            char inputButton;
            if (selectedMusic.Style == "FREE")
            {
                inputRight = 0;
                inputButton = '\0';
            }
            else
            {
                Track sameMusic = _trackList.Find(x => x.Title == selectedMusic.Title);
                string selectedButton = selectedMusic.Style.Substring(0, 2);
                var difficulties = new List<string> { "NM", "HD", "MX", "SC" };
                int a = difficulties.FindIndex(x => x == selectedMusic.Style.Substring(2, 2));
                var styles = new List<string>();

                for (int i = 0; i <= a; i++)
                {
                    styles.Add($"{selectedButton}{difficulties[i]}");
                }
                var list2 = from pattern in sameMusic.Patterns
                            where styles.Contains(pattern.Key)
                            select pattern.Value;
                int subCount = list2.Count(x => x == 0);
                inputRight = a - subCount;

                inputButton = selectedButton[0];
            }

            var inputCommand = new InputCommand()
            {
                Initial = Char.ToUpper(inputInitial),
                VerticalInputCount = inputVertical,
                ButtonTune = inputButton,
                RightInputCount = inputRight,
                IsAlphabet = isAlphabet,
                IsForward = isForward
            };

            return inputCommand;
        }

        public void Select(InputCommand inputCommand)
        {
            char initial = inputCommand.Initial;
            int vertical = inputCommand.VerticalInputCount;
            char button = inputCommand.ButtonTune;
            int right = inputCommand.RightInputCount;
            bool alphabet = inputCommand.IsAlphabet;
            bool forward = inputCommand.IsForward;
            int delay = inputCommand.Delay;
            bool starts = inputCommand.Starts;
            byte direction;


            void Input(byte key)
            {
                keybd_event(key, 0x45, 0x00, UIntPtr.Zero);
                keybd_event(key, 0x45, 0x02, UIntPtr.Zero);
                Thread.Sleep(delay);
            }

            if (forward)
            {
                direction = 40; // DOWN
            }
            else
            {
                direction = 38; // UP
            }

            Input(33); // PAGE UP
            Input((byte)initial);
            if (alphabet == false && forward)
            {
                Input(33); // PAGE UP
                Input(33); // PAGE UP
                Input(34); // PAGE DOWN
            }
            for(int i = 0; i < vertical; i++)
            {
                Input(direction);
            }

            if (button != '\0')
            {
                Input((byte)button);
                for (int i = 0; i < right; i++)
                {
                    Input(39); // RIGHT
                }
            }

            if (starts)
            {
                int startDelay = 800 - delay * (right + 1);
                startDelay = startDelay < 0 ? 0 : startDelay;

                Thread.Sleep(startDelay);
                Input(116); // F5
            }
        }
        #endregion

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
