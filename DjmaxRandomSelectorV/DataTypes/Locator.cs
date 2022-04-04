using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes
{
    public class Locator : IProvider
    {
        private readonly bool startsAutomatically;

        private const byte PageUp = 33;
        private const byte PageDown = 34;
        private const byte UpArrow = 38;
        private const byte RightArrow = 39;
        private const byte DownArrow = 40;
        private const byte F5 = 116;

        public Locator(bool startsAutomatically)
        {
            this.startsAutomatically = startsAutomatically;
        }

        public void Provide(Music selectedMusic, List<Track> trackList, int delay)
        {
            // Check if title starts with alphabet or not
            char initial = selectedMusic.Title[0];
            bool isAlphabet = Regex.IsMatch(initial.ToString(), "[a-z]", RegexOptions.IgnoreCase);

            // Create List of Tracks that have same initial with selected music
            List<Track> sameInitialList;
            if (isAlphabet)
            {
                var list = from track in trackList
                           let t = track.Title.Substring(0, 1)
                           where string.Compare(t, initial.ToString(), true) == 0
                           select track;
                sameInitialList = list.ToList();
            }
            else
            {
                var list = from track in trackList
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
                    inputInitial = initial;
                else
                    inputInitial = 'a';

                inputVertical = whereIsIt;
            }
            else
            {
                if (isAlphabet)
                    inputInitial = (char)(initial + 1);
                else
                    inputInitial = 'a';

                inputVertical = count - whereIsIt;
            }

            int inputRight;
            char inputButton;
            if (selectedMusic.Style.Equals("FREE"))
            {
                inputRight = 0;
                inputButton = '\0';
            }
            else
            {
                Track sameMusic = trackList.Find(x => x.Title.Equals(selectedMusic.Title));
                string selectedButton = selectedMusic.Style.Substring(0, 2);
                var difficulties = new List<string> { "NM", "HD", "MX", "SC" };
                int a = difficulties.FindIndex(x => x.Equals(selectedMusic.Style.Substring(2, 2)));
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

            // Input
            byte direction;
            void Input(byte key)
            {
                keybd_event(key, 0x45, 0x00, UIntPtr.Zero);
                keybd_event(key, 0x45, 0x02, UIntPtr.Zero);
                Thread.Sleep(delay);
            }

            if (isForward)
                direction = DownArrow;
            else
                direction = UpArrow;

            Input(PageUp);
            Input((byte)initial);
            if (!isAlphabet && isForward)
            {
                Input(PageUp);
                Input(PageUp);
                Input(PageDown);
            }
            for (int i = 0; i < inputVertical; i++)
            {
                Input(direction);
            }

            if (inputButton != '\0')
            {
                Input((byte)inputButton);
                for (int i = 0; i < inputRight; i++)
                {
                    Input(RightArrow);
                }
            }

            if (startsAutomatically)
            {
                int startDelay = 800 - delay * (inputRight + 1);
                startDelay = startDelay < 0 ? 0 : startDelay;
                Thread.Sleep(startDelay);
                Input(F5);
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
