﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DjmaxRandomSelectorV
{
    public class WindowTitleHelper
    {
        private const string DjmaxTitle = "DJMAX RESPECT V";

        public bool EqualsDjmax()
        {
            return GetActiveWindowTitle().Equals(DjmaxTitle);
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return string.Empty;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
