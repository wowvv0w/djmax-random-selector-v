using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.RandomSelector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class Locator : IProvider
    {
        private readonly bool _startsAutomatically;
        private static bool _isKeyMapInitialized = false;
        private static Dictionary<string, ushort> KeyMap = new Dictionary<string, ushort>
        {
            {"1", 0x02},
            {"2", 0x03},
            {"3", 0x04},
            {"4", 0x05},
            {"5", 0x06},
            {"6", 0x07},
            {"7", 0x08},
            {"8", 0x09},
            {"9", 0x0A},
            {"0", 0x0B},
            {"q", 0x10},
            {"w", 0x11},
            {"e", 0x12},
            {"r", 0x13},
            {"t", 0x14},
            {"y", 0x15},
            {"u", 0x16},
            {"i", 0x17},
            {"o", 0x18},
            {"p", 0x19},
            {"a", 0x1E},
            {"s", 0x1F},
            {"d", 0x20},
            {"f", 0x21},
            {"g", 0x22},
            {"h", 0x23},
            {"j", 0x24},
            {"k", 0x25},
            {"l", 0x26},
            {";", 0x27},
            {"z", 0x2C},
            {"x", 0x2D},
            {"c", 0x2E},
            {"v", 0x2F},
            {"b", 0x30},
            {"n", 0x31},
            {"m", 0x32},
            {"f5", 0x3F},
            {"shiftleft", 0x2A},
            {"shiftright", 0x36},
            {"pageup", 0xC9 + 1024},
            {"pagedown", 0xD1 + 1024},
        };

        public Locator(bool startsAutomatically)
        {
            this._startsAutomatically = startsAutomatically;
            if (!_isKeyMapInitialized)
            {
                KeyMap.Add("left", (ushort)MapVirtualKey(0x25, 0));
                KeyMap.Add("up", (ushort)MapVirtualKey(0x26, 0));
                KeyMap.Add("right", (ushort)MapVirtualKey(0x27, 0));
                KeyMap.Add("down", (ushort)MapVirtualKey(0x28, 0));
                _isKeyMapInitialized = true;
            }
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
            inputInitial = char.ToLower(inputInitial);

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
            void Input(ushort key, bool isArrowKey = false)
            {
                KeyDown(key, isArrowKey);
                Thread.Sleep(delay);
                KeyUp(key, isArrowKey);
                Thread.Sleep(delay);
            }

            string direction = isForward ? "down" : "up";

            ResetMusicCursor();
            if (isAlphabet || !isForward)
            {
                Input(KeyMap[inputInitial.ToString()]);
            }

            for (int i = 0; i < inputVertical; i++)
            {
                Input(KeyMap[direction], true);
            }

            if (inputButton != '\0')
            {
                BtnSelect(inputButton);
                for (int i = 0; i < inputRight; i++)
                {
                    Input(KeyMap["right"], true);
                }
            }

            if (_startsAutomatically)
            {
                int startDelay = 800 - delay * (inputRight + 1);
                startDelay = startDelay < 0 ? 0 : startDelay;
                Thread.Sleep(startDelay);
                Input(KeyMap["f5"]);
            }
        }

        private static void BtnSelect(char btn)
        {
            ushort scancode = 0x52;
            if (btn == '4') scancode = 0x4B;
            else if (btn == '5') scancode = 0x4C;
            else if (btn == '6') scancode = 0x4D;
            else if (btn == '8') scancode = 0x48;

            KeyDown(0x1D);
            Thread.Sleep(20);
            KeyDown(scancode);
            Thread.Sleep(20);
            KeyUp(scancode);
            Thread.Sleep(20);
            KeyUp(0x1D);
            Thread.Sleep(20);
        }

        private static void ResetMusicCursor()
        {
            KeyDown(KeyMap["shiftright"]);
            Thread.Sleep(20);
            KeyDown(KeyMap["shiftleft"]);
            Thread.Sleep(20);
            KeyUp(KeyMap["shiftright"]);
            Thread.Sleep(20);
            KeyUp(KeyMap["shiftleft"]);
            Thread.Sleep(20);
        }

        private static bool KeyDown(ushort ScanCode, bool isArrowKey = false)
        {
            // Original code from https://github.com/learncodebygaming/pydirectinput
            // Copyright(c) 2020 Ben Johnson
            uint insertedEvents = 0;
            uint expectedEvents = 1;
            uint Flags = 0x0008;    // KEYEVENTF_SCANCODE
            INPUT input = new INPUT { Type = 1 };
            input.Data.Keyboard = new KEYBDINPUT
            {
                Vk = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            if (isArrowKey)
            {
                Flags |= 0x0001;    // KEYEVENTF_EXTENDEDKEY
                if (GetKeyState(0x90) != 0)
                {
                    INPUT input2 = new INPUT { Type = 1 };
                    input2.Data.Keyboard = new KEYBDINPUT
                    {
                        Vk = 0,
                        Scan = 0xE0,
                        Flags = 0x0008,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    };
                    INPUT[] inputs2 = new INPUT[] { input2 };
                    expectedEvents = 2;
                    insertedEvents += SendInput(1, inputs2, Marshal.SizeOf(typeof(INPUT)));
                }
            }
            input.Data.Keyboard.Scan = ScanCode;
            input.Data.Keyboard.Flags = Flags;

            INPUT[] inputs = new INPUT[] { input };
            insertedEvents += SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
            return insertedEvents == expectedEvents;
        }

        private static bool KeyUp(ushort ScanCode, bool isArrowKey = false)
        {
            uint insertedEvents = 0;
            uint expectedEvents = 1;
            uint Flags = 0x0008 | 0x0002;       // KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP
            INPUT input = new INPUT { Type = 1 };
            input.Data.Keyboard = new KEYBDINPUT
            {
                Vk = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            if (isArrowKey)
            {
                Flags |= 0x0001;    // KEYEVENTF_EXTENDEDKEY
            }

            input.Data.Keyboard.Scan = ScanCode;
            input.Data.Keyboard.Flags = Flags;

            INPUT[] inputs = new INPUT[] { input };
            insertedEvents += SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));

            if (isArrowKey && GetKeyState(0x90) != 0)
            {
                INPUT input2 = new INPUT { Type = 1 };
                input2.Data.Keyboard = new KEYBDINPUT
                {
                    Vk = 0,
                    Scan = 0xE0,
                    Flags = 0x0008 | 0x0002,
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
                INPUT[] inputs2 = new INPUT[] { input2 };
                expectedEvents = 2;
                insertedEvents += SendInput(1, inputs2, Marshal.SizeOf(typeof(INPUT)));
            }
            return insertedEvents == expectedEvents;
        }

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(int wCode, int wMapType);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public uint Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }
    }
}
