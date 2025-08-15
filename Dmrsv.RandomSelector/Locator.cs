using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Dmrsv.RandomSelector
{
    public static class Locator
    {
        private const uint MAPVK_VK_TO_VSC = 0;
        //private const uint MAPVK_VSC_TO_VK = 1;
        //private const uint MAPVK_VK_TO_CHAR = 2;
        //private const uint MAPVK_VSC_TO_VK_EX = 3;
        //private const uint MAPVK_VK_TO_VSC_EX = 4;

        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;
        //private const uint KEYEVENTF_UNICODE = 0x0004;

        private readonly static Dictionary<string, ushort> _keyMap = new()
        {
            ["4"] = 0x4B, ["5"] = 0x4C, ["6"] = 0x4D, ["8"] = 0x48, // numpad numbers
            ["q"] = 0x10, ["w"] = 0x11, ["e"] = 0x12, ["r"] = 0x13, ["t"] = 0x14,
            ["y"] = 0x15, ["u"] = 0x16, ["i"] = 0x17, ["o"] = 0x18, ["p"] = 0x19,
            ["a"] = 0x1E, ["s"] = 0x1F, ["d"] = 0x20, ["f"] = 0x21, ["g"] = 0x22,
            ["h"] = 0x23, ["j"] = 0x24, ["k"] = 0x25, ["l"] = 0x26, [";"] = 0x27,
            ["z"] = 0x2C, ["x"] = 0x2D, ["c"] = 0x2E, ["v"] = 0x2F, ["b"] = 0x30,
            ["n"] = 0x31, ["m"] = 0x32,
            ["f5"] = 0x3F, ["ctrl"] = 0x1D,
            ["shiftleft"] = 0x2A, ["shiftright"] = 0x36,
            ["pageup"] = 0xC9 + 1024, ["pagedown"] = 0xD1 + 1024,
            ["left"] = (ushort)MapVirtualKey(0x25, MAPVK_VK_TO_VSC),
            ["up"] = (ushort)MapVirtualKey(0x26, MAPVK_VK_TO_VSC),
            ["right"] = (ushort)MapVirtualKey(0x27, MAPVK_VK_TO_VSC),
            ["down"] = (ushort)MapVirtualKey(0x28, MAPVK_VK_TO_VSC),
        };

        private readonly static HashSet<ushort> NumberVSC = new() { _keyMap["4"], _keyMap["5"], _keyMap["6"], _keyMap["8"] };
        private readonly static HashSet<ushort> ArrowVSC = new() { _keyMap["left"], _keyMap["up"], _keyMap["right"], _keyMap["down"] };

        public static IReadOnlyDictionary<int, LocationInfo?> MakeLocationMap(IEnumerable<Track> tracks)
        {
            char GetGroup(Track t)
            {
                char initial = char.ToLower(t.Title[0]);
                return Regex.IsMatch(initial.ToString(), "[a-z]", RegexOptions.IgnoreCase) ? initial : '#';
            }

            var groupByInitial = tracks.Where(t => t.IsPlayable)
                                       .OrderBy(t => t.Title, new TitleComparer())
                                       .ThenByDescending(t => t.Id == 170 || t.Id == 267 ? t.Id : 0)
                                       .GroupBy(t => GetGroup(t))
                                       .ToDictionary(g => g.Key, g => g.ToList());

            int GetIndex(Track t)
            {
                char initial = char.ToLower(GetGroup(t));
                int index = groupByInitial[initial].IndexOf(t);
                int count = groupByInitial[initial].Count;
                return index <= (count - 1) / 2 || "wxyzWXYZ".Contains(initial) ? index : index - count;
            }

            LocationInfo? GetLocationInfo(Track t)
            {
                if (!t.IsPlayable)
                {
                    return null;
                }
                return new LocationInfo()
                {
                    TrackId = t.Id,
                    Group = GetGroup(t),
                    Index = GetIndex(t),
                    DifficultyOrder = t.Patterns
                                       .GroupBy(p => p.Button)
                                       .SelectMany(g => g.Select((p, order) => new { p.Style, order }))
                                       .ToDictionary(o => o.Style, o => o.order)
                };
            }

            return tracks.ToDictionary(t => t.Id, t => GetLocationInfo(t));
        }

        public static void Locate(LocationInfo location, string targetStyle, int inputInterval, bool autoStart)
        {
            if (location is null)
            {
                return;
            }
            // Reset cursor
            Press("shiftright", inputInterval);
            Press("shiftleft", inputInterval);

            char group = location.Group;
            if (location.Index < 0)
            {
                group = group == '#' ? 'a' : (group == 'z' ? '#' : (char)(group + 1));
            }
            if (group != '#')
            {
                Press(group.ToString(), inputInterval);
            }

            string arrow = location.Index < 0 ? "up" : "down";
            int distance = Math.Abs(location.Index);
            Press(arrow, inputInterval, distance);

            if (!string.IsNullOrEmpty(targetStyle) && location.DifficultyOrder.ContainsKey(targetStyle))
            {
                int difficultyOrder = location.DifficultyOrder[targetStyle];
                string button = targetStyle[..1];
                Press(button, inputInterval);
                Press("right", inputInterval, difficultyOrder);
            }

            if (autoStart)
            {
                Thread.Sleep(800);
                Press("f5", inputInterval);
            }
        }

        private static bool Press(string key, int interval, int nTimes = 1)
        {
            // Original code from https://github.com/learncodebygaming/pydirectinput
            // Copyright(c) 2020 Ben Johnson
            ushort scanCode = _keyMap[key];
            bool isNumber = NumberVSC.Contains(scanCode);
            int expectedPresses = nTimes;
            int completedPresses = 0;
            int ctrlStatus = 0; // for number key presses

            if (isNumber)
            {
                // In order to select buttons in online mode, press ctrl when the key is number
                if (SendKeyDown(_keyMap["ctrl"]))
                {
                    ++ctrlStatus;
                }
                Thread.Sleep(interval);
            }
            for (int i = 0; i < nTimes; ++i)
            {
                bool downed = SendKeyDown(scanCode);
                Thread.Sleep(interval);
                bool upped = SendKeyUp(scanCode);
                Thread.Sleep(interval);
                if (downed && upped)
                {
                    ++completedPresses;
                }
            }
            if (isNumber)
            {
                if (SendKeyUp(_keyMap["ctrl"]))
                {
                    --ctrlStatus;
                }
                Thread.Sleep(interval);
            }

            return completedPresses == expectedPresses && ctrlStatus == 0;
        }

        private static bool SendKeyDown(ushort scanCode)
        {
            // Original code from https://github.com/learncodebygaming/pydirectinput
            // Copyright(c) 2020 Ben Johnson
            uint insertedEvents = 0;
            uint expectedEvents = 1;
            uint flags = KEYEVENTF_SCANCODE;
            INPUT input = new INPUT { Type = 1 };
            input.Data.Keyboard = new KEYBDINPUT
            {
                Vk = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            if (ArrowVSC.Contains(scanCode))
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
                if (GetKeyState(0x90) != 0)
                {
                    INPUT input2 = new INPUT { Type = 1 };
                    input2.Data.Keyboard = new KEYBDINPUT
                    {
                        Vk = 0,
                        Scan = 0xE0,
                        Flags = KEYEVENTF_SCANCODE,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    };
                    INPUT[] inputs2 = new INPUT[] { input2 };
                    expectedEvents = 2;
                    insertedEvents += SendInput(1, inputs2, Marshal.SizeOf(typeof(INPUT)));
                }
            }
            input.Data.Keyboard.Scan = scanCode;
            input.Data.Keyboard.Flags = flags;

            INPUT[] inputs = new INPUT[] { input };
            insertedEvents += SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
            return insertedEvents == expectedEvents;
        }

        private static bool SendKeyUp(ushort scanCode)
        {
            // Original code from https://github.com/learncodebygaming/pydirectinput
            // Copyright(c) 2020 Ben Johnson
            uint insertedEvents = 0;
            uint expectedEvents = 1;
            uint flags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;
            INPUT input = new INPUT { Type = 1 };
            input.Data.Keyboard = new KEYBDINPUT
            {
                Vk = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            if (ArrowVSC.Contains(scanCode))
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
            }

            input.Data.Keyboard.Scan = scanCode;
            input.Data.Keyboard.Flags = flags;

            INPUT[] inputs = new INPUT[] { input };
            insertedEvents += SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));

            if (ArrowVSC.Contains(scanCode) && GetKeyState(0x90) != 0)
            {
                INPUT input2 = new INPUT { Type = 1 };
                input2.Data.Keyboard = new KEYBDINPUT
                {
                    Vk = 0,
                    Scan = 0xE0,
                    Flags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP,
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
        private static extern uint MapVirtualKey(uint wCode, uint wMapType);
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }
    }
}
