using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace DjmaxRandomSelectorV.Models
{
    public class Selector
    {
        public static bool IsFilterChanged { get; set; }
        public static List<Track> AllTrackList { get; set; }
        public static List<Track> TrackList { get; set; }
        private static List<Music> MusicList { get; set; }

        public static bool CanStart { get; set; } = true;
        public static void Start()
        {
            CanStart = false;
            if (IsFilterChanged)
            {
                SiftOut();
                IsFilterChanged = false;
            }

            ArrayList inputList = Pick();

            Console.WriteLine($"{inputList[0]} {inputList[1]} {inputList[2]} {inputList[3]}");
            Console.WriteLine("--------------------------");

            //Select(inputList);
            CanStart = true;
        }
        private static void SiftOut()
        {
            List<string> Styles = new List<string>();
            foreach(string button in Filter.ButtonTunes)
            {
                foreach(string difficulty in Filter.Difficulties)
                {
                    Styles.Add($"{button}{difficulty}");
                }
            }
            var musicList = from track in TrackList
                            from pattern in track.Patterns
                            where Filter.Categories.Contains(track.Category)
                            && Styles.Contains(pattern.Key)
                            && pattern.Value >= Filter.Levels[0]
                            && pattern.Value <= Filter.Levels[1]
                            select new Music
                            {
                                Title = track.Title,
                                Style = pattern.Key,
                                Level = pattern.Value
                            };
            MusicList = musicList.ToList();
        }

        private static ArrayList Pick()
        {
            // Select randomly
            var random = new Random();
            var index = random.Next(MusicList.Count() - 1);
            var selectedMusic = MusicList[index];
            Console.WriteLine($"{selectedMusic.Title} {selectedMusic.Style} {selectedMusic.Level}");

            // Check if title starts with alphabet or not
            char initial = selectedMusic.Title[0];
            bool isAlphabet = Regex.IsMatch(initial.ToString(), "[a-z]", RegexOptions.IgnoreCase);

            // Create List of Tracks that have same initial with selected music
            List<Track> sameInitialList;
            if (isAlphabet)
            {
                var list = from track in TrackList
                           let t = track.Title.Substring(0, 1)
                           where String.Compare(t, initial.ToString()) == 0
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
            int count = sameInitialList.Count();
            bool isForward = whereIsIt <= Math.Ceiling((double)count / 2) || "wxyz".Contains(initial);
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
            List<string> difficulties = new List<string> { "NM", "HD", "MX", "SC" };
            int a = difficulties.FindIndex(x => x == selectedMusic.Style.Substring(2, 2));
            List<string> styles = new List<string>();
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

            return new ArrayList() 
            { 
                inputButton, Char.ToUpper(inputInitial), inputVertical, inputRight,
                isAlphabet, isForward 
            };
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static void Select(ArrayList inputList)
        {
            char button = (char)inputList[0];
            char initial = (char)inputList[1];
            int vertical = (int)inputList[2];
            int right = (int)inputList[3];
            bool alphabet = (bool)inputList[4];
            bool forward = (bool)inputList[5];
            var delay = Filter.InputDelay;
            byte direction;


            void Input(byte key)
            {
                keybd_event(key, 0x45, 0x1, UIntPtr.Zero);
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

            Input((byte)button);
            Input((byte)initial);
            if (alphabet == false && forward)
            {
                Input(33); // PAGE UP
                Input(33);
                Input(34); // PAGE DOWN
            }
            for(int i = 0; i < vertical; i++)
            {
                Input(direction);
            }
            for(int i = 0; i < right; i++)
            {
                Input(39); // RIGHT
            }
            
        }
    }
}
