using System;
using System.Runtime.InteropServices;

namespace TarnishedTool.Utilities
{
    internal static class User32
    {
        public static readonly IntPtr HwndTopmost = new IntPtr(-1);
        public static readonly IntPtr HwndNoTopmost = new IntPtr(-2);
        public const uint SwpNosize = 0x0001;
        public const uint SwpNomove = 0x0002;
        public const uint SwpNoactivate = 0x0010;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            uint uFlags);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        
        public static void SetTopmost(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNoactivate);
        }
        
        public static void RemoveTopmost(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HwndNoTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNoactivate);
        }
        
    }
}