using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DjmaxRandomSelectorV.Models
{
    public class Selector
    {
        public static bool IsFilterChanged { get; set; } = true;
        public static List<Track> AllTrackList { get; set; }
        public static List<Track> TrackList { get; set; }
        private static List<Music> MusicList { get; set; }
        private static List<string> TitleList { get; set; }
        public static bool CanStart { get; set; } = true;

        public static void SiftOut(Filter filter)
        {
            List<string> Styles = new List<string>();
            foreach(string button in filter.ButtonTunes)
            {
                foreach(string difficulty in filter.Difficulties)
                {
                    Styles.Add($"{button}{difficulty}");
                }
            }
            var musicList = from track in TrackList
                            from pattern in track.Patterns
                            where filter.Categories.Contains(track.Category)
                            && Styles.Contains(pattern.Key)
                            && pattern.Value >= filter.Levels[0]
                            && pattern.Value <= filter.Levels[1]
                            select new Music
                            {
                                Title = track.Title,
                                Style = pattern.Key,
                                Level = pattern.Value
                            };
            MusicList = musicList.ToList();

            var titleList = from music in MusicList
                            select music.Title;
            TitleList = titleList.Distinct().ToList();
        }


        public static List<string> CheckRecents(List<string> recents)
        {
            if (recents.Count >= TitleList.Count)
            {
                try
                {
                    recents.RemoveRange(0, recents.Count - TitleList.Count + 1);
                }
                catch (ArgumentException)
                {
                }
            }
            else if (recents.Count > 5)
            {
                recents.RemoveAt(0);
            }

            return recents;
        }

        public static Music Pick(List<string> recents)
        {
            var musicList = (from music in MusicList
                             where !recents.Contains(music.Title)
                             select music).ToList();

            var random = new Random();
            var index = random.Next(musicList.Count - 1);
            var selectedMusic = musicList[index];

            return selectedMusic;
        }

        public static InputCommand Find(Music selectedMusic) 
        {
            // Check if title starts with alphabet or not
            char initial = selectedMusic.Title[0];
            bool isAlphabet = Regex.IsMatch(initial.ToString(), "[a-z]", RegexOptions.IgnoreCase);

            // Create List of Tracks that have same initial with selected music
            List<Track> sameInitialList;
            if (isAlphabet)
            {
                var list = from track in TrackList
                           let t = track.Title.Substring(0, 1)
                           where String.Compare(t, initial.ToString(), true) == 0
                           select track;
                sameInitialList = list.ToList();
            }
            else
            {
                var list = from track in TrackList
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

            // Find right arrow input count
            Track sameMusic = TrackList.Find(x => x.Title == selectedMusic.Title);
            string selectedButton = selectedMusic.Style.Substring(0, 2);
            var difficulties = new List<string> { "NM", "HD", "MX", "SC" };
            int a = difficulties.FindIndex(x => x == selectedMusic.Style.Substring(2, 2));
            var styles = new List<string>();

            for(int i = 0; i <= a; i++)
            {
                styles.Add($"{selectedButton}{difficulties[i]}");
            }
            var list2 = from pattern in sameMusic.Patterns
                       where styles.Contains(pattern.Key)
                       select pattern.Value;
            int subCount = list2.Count(x => x == 0);
            int inputRight = a - subCount;

            char inputButton = selectedButton[0];

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

        public static void Select(InputCommand inputCommand, int delay)
        {
            char initial = inputCommand.Initial;
            int vertical = inputCommand.VerticalInputCount;
            char button = inputCommand.ButtonTune;
            int right = inputCommand.RightInputCount;
            bool alphabet = inputCommand.IsAlphabet;
            bool forward = inputCommand.IsForward;
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

            Input((byte)button);
            for(int i = 0; i < right; i++)
            {
                Input(39); // RIGHT
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
