using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;


namespace TarnishedTool.Behaviors
{
    public static class WindowResizeBehavior
    {
        public static readonly DependencyProperty EnableCustomResizeProperty =
            DependencyProperty.RegisterAttached(
                "EnableCustomResize",
                typeof(bool),
                typeof(WindowResizeBehavior),
                new PropertyMetadata(false, OnEnableCustomResizeChanged));

        public static bool GetEnableCustomResize(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableCustomResizeProperty);
        }

        public static void SetEnableCustomResize(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableCustomResizeProperty, value);
        }

        private static void OnEnableCustomResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (bool)e.NewValue)
            {
                window.SourceInitialized += Window_SourceInitialized;
            }
        }

        private static void Window_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window)sender;
            var hwnd = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int
                wmNchittest = 0x0084;
            
            const int
                wmGetminmaxinfo = 0x0024;

            switch (msg)
            {
                case wmNchittest:
                    handled = true;
                    return HitTestNca(hwnd, wParam, lParam);

                case wmGetminmaxinfo:
                    HandleGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private static IntPtr HitTestNca(IntPtr hwnd, IntPtr wParam, IntPtr lParam)
        {
            // Get the point coordinates
            int x = (short)(lParam.ToInt32() & 0xFFFF);
            int y = (short)(lParam.ToInt32() >> 16);

            // Get the window rectangle
            Rect rect;
            GetWindowRect(hwnd, out rect);

            // Define the resize border thickness
            const int resizeBorder = 8;

            // Calculate relative position
            int relativeX = x - rect.Left;
            int relativeY = y - rect.Top;
            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            // Check for corners
            if (relativeX < resizeBorder && relativeY < resizeBorder)
                return (IntPtr)Httopleft;
            if (relativeX > windowWidth - resizeBorder && relativeY < resizeBorder)
                return (IntPtr)Httopright;
            if (relativeX < resizeBorder && relativeY > windowHeight - resizeBorder)
                return (IntPtr)Htbottomleft;
            if (relativeX > windowWidth - resizeBorder && relativeY > windowHeight - resizeBorder)
                return (IntPtr)Htbottomright;

            // Check for edges
            if (relativeY < resizeBorder)
                return (IntPtr)Httop;
            if (relativeY > windowHeight - resizeBorder)
                return (IntPtr)Htbottom;
            if (relativeX < resizeBorder)
                return (IntPtr)Htleft;
            if (relativeX > windowWidth - resizeBorder)
                return (IntPtr)Htright;

            // Default to client area
            return (IntPtr)Htclient;
        }

        private static void HandleGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<Minmaxinfo>(lParam);
            var monitor = MonitorFromWindow(hwnd, MonitorDefaulttonearest);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new Monitorinfo();
                GetMonitorInfo(monitor, monitorInfo);

                var workArea = monitorInfo.rcWork;
                var monitorArea = monitorInfo.rcMonitor;

                // Maximized window position
                mmi.ptMaxPosition.x = Math.Abs(workArea.Left - monitorArea.Left);
                mmi.ptMaxPosition.y = Math.Abs(workArea.Top - monitorArea.Top);

                // Maximized window size 
                mmi.ptMaxSize.x = Math.Abs(workArea.Right - workArea.Left);
                mmi.ptMaxSize.y = Math.Abs(workArea.Bottom - workArea.Top);
            }


            var source = HwndSource.FromHwnd(hwnd);
            if (source?.RootVisual is Window window)
            {
                var dpi = VisualTreeHelper.GetDpi(window);

                mmi.ptMinTrackSize.x = (int)(window.MinWidth * dpi.DpiScaleX);
                mmi.ptMinTrackSize.y = (int)(window.MinHeight * dpi.DpiScaleY);
                
                if (!double.IsInfinity(window.MaxWidth))
                    mmi.ptMaxTrackSize.x = (int)(window.MaxWidth * dpi.DpiScaleX);

                if (!double.IsInfinity(window.MaxHeight))
                    mmi.ptMaxTrackSize.y = (int)(window.MaxHeight * dpi.DpiScaleY);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        #region Native Constants

        private const int Httopleft = 13;
        private const int Httop = 12;
        private const int Httopright = 14;
        private const int Htleft = 10;
        private const int Htright = 11;
        private const int Htbottomleft = 16;
        private const int Htbottom = 15;
        private const int Htbottomright = 17;
        private const int Htclient = 1;
        private const uint MonitorDefaulttonearest = 2;

        #endregion

        #region Native Structures

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Minmaxinfo
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class Monitorinfo
        {
            public int cbSize = Marshal.SizeOf(typeof(Monitorinfo));
            public Rect rcMonitor;
            public Rect rcWork;
            public uint dwFlags;
        }

        #endregion

        #region Native Methods

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, Monitorinfo lpmi);

        #endregion
    }
}