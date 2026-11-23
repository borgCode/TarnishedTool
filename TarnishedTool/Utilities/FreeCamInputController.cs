using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TarnishedTool.Interfaces;
using static TarnishedTool.Memory.Offsets;
using Application = System.Windows.Application;

namespace TarnishedTool.Utilities
{
    public class FreeCamInputController(IMemoryService memoryService) : NativeWindow
    {
        private const int WmInput = 0x00FF;
        private const int RidInput = 0x10000003;
        private const int RimTypeMouse = 0;
        private const int RidevInputsink = 0x00000100;

        private const int VkW = 0x57;
        private const int VkS = 0x53;
        private const int VkA = 0x41;
        private const int VkD = 0x44;
        private const int VkSpace = 0x20;
        private const int VkControl = 0x11;
        private const int VkShift = 0x10;

        private const float BaseSpeed = 7f;
        private const float FastSpeed = 20f;
        private const float MouseSensitivity = 0.0025f;
        private const float PitchLimit = 1.55f;

        [StructLayout(LayoutKind.Sequential)]
        private struct RawInputDevice
        {
            public ushort UsUsagePage;
            public ushort UsUsage;
            public int DwFlags;
            public IntPtr HwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawInputHeader
        {
            public int DwType;
            public int DwSize;
            public IntPtr HDevice;
            public IntPtr WParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawMouse
        {
            public ushort UsFlags;
            public ushort UsButtonFlags;
            public ushort UsButtonData;
            public uint UlRawButtons;
            public int LLastX;
            public int LLastY;
            public uint UlExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawInput
        {
            public RawInputHeader Header;
            public RawMouse Mouse;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        private int _accumDx;
        private int _accumDy;
        private bool _rawInputRegistered;
        private volatile bool _running;
        private Thread _thread;

        public void Start()
        {
            if (_running) return;
            EnsureRawInputRegistered();
            Interlocked.Exchange(ref _accumDx, 0);
            Interlocked.Exchange(ref _accumDy, 0);
            _running = true;
            _thread = new Thread(UpdateLoop) { IsBackground = true };
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _thread = null;
        }

        private void EnsureRawInputRegistered()
        {
            if (_rawInputRegistered) return;
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (_rawInputRegistered) return;
                CreateHandle(new CreateParams { Parent = new IntPtr(-3) });
                var rid = new RawInputDevice[]
                {
                    new()
                    {
                        UsUsagePage = 0x01,
                        UsUsage = 0x02,
                        DwFlags = RidevInputsink,
                        HwndTarget = Handle
                    }
                };
                RegisterRawInputDevices(rid, 1, (uint)Marshal.SizeOf<RawInputDevice>());
                _rawInputRegistered = true;
            });
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmInput && _running)
            {
                uint size = 0;
                uint headerSize = (uint)Marshal.SizeOf<RawInputHeader>();
                GetRawInputData(m.LParam, RidInput, IntPtr.Zero, ref size, headerSize);
                if (size > 0 && size <= 256)
                {
                    var buf = Marshal.AllocHGlobal((int)size);
                    try
                    {
                        if (GetRawInputData(m.LParam, RidInput, buf, ref size, headerSize) == size)
                        {
                            var raw = Marshal.PtrToStructure<RawInput>(buf);
                            if (raw.Header.DwType == RimTypeMouse)
                            {
                                Interlocked.Add(ref _accumDx, raw.Mouse.LLastX);
                                Interlocked.Add(ref _accumDy, raw.Mouse.LLastY);
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buf);
                    }
                }
            }

            base.WndProc(ref m);
        }

        private static bool IsKeyDown(int vk) => (User32.GetAsyncKeyState(vk) & 0x8000) != 0;

        private bool IsGameForeground()
        {
            var proc = memoryService.TargetProcess;
            if (proc == null || proc.HasExited) return false;
            User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out var fgPid);
            return fgPid == (uint)proc.Id;
        }

        private void UpdateLoop()
        {
            var sw = Stopwatch.StartNew();
            var lastTime = sw.Elapsed.TotalSeconds;
            var wasActive = false;
            float yaw = 0f, pitch = 0f;

            while (_running)
            {
                var now = sw.Elapsed.TotalSeconds;
                var dt = (float)(now - lastTime);
                lastTime = now;
                if (dt > 0.1f) dt = 0.016f;

                var dx = Interlocked.Exchange(ref _accumDx, 0);
                var dy = Interlocked.Exchange(ref _accumDy, 0);

                if (!memoryService.IsAttached || !IsGameForeground())
                {
                    wasActive = false;
                    Thread.Sleep(16);
                    continue;
                }

                var gameRend = memoryService.FollowPointers(FieldArea.Base, [FieldArea.GameRend], true);
                if (gameRend == 0 || memoryService.Read<byte>(gameRend + FieldArea.CamMode) != 1)
                {
                    wasActive = false;
                    Thread.Sleep(16);
                    continue;
                }

                var dbgCam = memoryService.Read<nint>(gameRend + FieldArea.CSDebugCam);
                if (dbgCam == 0)
                {
                    Thread.Sleep(16);
                    continue;
                }

                var mat = memoryService.ReadBytes(dbgCam + 0x10, 0x40);
                if (mat == null || mat.Length < 0x40)
                {
                    Thread.Sleep(16);
                    continue;
                }

                var fx = BitConverter.ToSingle(mat, 32);
                var fy = BitConverter.ToSingle(mat, 36);
                var fz = BitConverter.ToSingle(mat, 40);
                var px = BitConverter.ToSingle(mat, 48);
                var py = BitConverter.ToSingle(mat, 52);
                var pz = BitConverter.ToSingle(mat, 56);

                if (!wasActive)
                {
                    yaw = (float)Math.Atan2(fx, fz);
                    pitch = (float)Math.Asin(Math.Max(-0.999f, Math.Min(0.999f, fy)));
                    wasActive = true;
                }

                var changed = false;
                if (dx != 0 || dy != 0)
                {
                    yaw += dx * MouseSensitivity;
                    pitch = Math.Max(-PitchLimit, Math.Min(PitchLimit, pitch - dy * MouseSensitivity));
                    changed = true;
                }

                var cosP = (float)Math.Cos(pitch);
                var sinP = (float)Math.Sin(pitch);
                var sinY = (float)Math.Sin(yaw);
                var cosY = (float)Math.Cos(yaw);

                var nfx = sinY * cosP;
                var nfy = sinP;
                var nfz = cosY * cosP;

                var nrx = cosY;
                var nrz = -sinY;

                var nux = nfy * nrz;
                var nuy = nfz * nrx - nfx * nrz;
                var nuz = -nfy * nrx;

                var move = (IsKeyDown(VkShift) ? FastSpeed : BaseSpeed) * dt;

                if (IsKeyDown(VkW)) { px += nfx * move; py += nfy * move; pz += nfz * move; changed = true; }
                if (IsKeyDown(VkS)) { px -= nfx * move; py -= nfy * move; pz -= nfz * move; changed = true; }
                if (IsKeyDown(VkD)) { px += nrx * move; pz += nrz * move; changed = true; }
                if (IsKeyDown(VkA)) { px -= nrx * move; pz -= nrz * move; changed = true; }
                if (IsKeyDown(VkSpace)) { py += move; changed = true; }
                if (IsKeyDown(VkControl)) { py -= move; changed = true; }

                if (changed)
                {
                    var outMat = new byte[0x40];
                    Array.Copy(BitConverter.GetBytes(nrx), 0, outMat, 0, 4);
                    Array.Copy(BitConverter.GetBytes(nrz), 0, outMat, 8, 4);
                    Array.Copy(BitConverter.GetBytes(nux), 0, outMat, 16, 4);
                    Array.Copy(BitConverter.GetBytes(nuy), 0, outMat, 20, 4);
                    Array.Copy(BitConverter.GetBytes(nuz), 0, outMat, 24, 4);
                    Array.Copy(BitConverter.GetBytes(nfx), 0, outMat, 32, 4);
                    Array.Copy(BitConverter.GetBytes(nfy), 0, outMat, 36, 4);
                    Array.Copy(BitConverter.GetBytes(nfz), 0, outMat, 40, 4);
                    Array.Copy(BitConverter.GetBytes(px), 0, outMat, 48, 4);
                    Array.Copy(BitConverter.GetBytes(py), 0, outMat, 52, 4);
                    Array.Copy(BitConverter.GetBytes(pz), 0, outMat, 56, 4);
                    Array.Copy(BitConverter.GetBytes(1f), 0, outMat, 60, 4);

                    memoryService.WriteBytes(dbgCam + 0x10, outMat);
                }

                Thread.Sleep(8);
            }
        }
    }
}
