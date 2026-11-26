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

        private const float BaseSpeed = 7f;
        private const float FastSpeed = 20f;
        private const float MouseSensitivity = 0.0025f;
        private const float TiltSpeed = 1.5f;
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

        private static bool IsKeyDown(int vk) => vk > 0 && (User32.GetAsyncKeyState(vk) & 0x8000) != 0;

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
            float yaw = 0f, pitch = 0f, roll = 0f;

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

                var rx = BitConverter.ToSingle(mat, 0);
                var ry = BitConverter.ToSingle(mat, 4);
                var rz = BitConverter.ToSingle(mat, 8);
                var ux = BitConverter.ToSingle(mat, 16);
                var uy = BitConverter.ToSingle(mat, 20);
                var uz = BitConverter.ToSingle(mat, 24);
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
                    var r0xInit = (float)Math.Cos(yaw);
                    var r0zInit = -(float)Math.Sin(yaw);
                    var dotR = rx * r0xInit + rz * r0zInit;
                    var dotU = ux * r0xInit + uz * r0zInit;
                    roll = (float)Math.Atan2(-dotU, dotR);
                    wasActive = true;
                }

                var cfg = SettingsManager.Default;
                var changed = false;

                if (dx != 0 || dy != 0)
                {
                    yaw += dx * MouseSensitivity;
                    pitch = Math.Max(-PitchLimit, Math.Min(PitchLimit, pitch - dy * MouseSensitivity));
                    changed = true;
                }

                if (IsKeyDown(cfg.FreeCamTiltLeftKey)) { roll += TiltSpeed * dt; changed = true; }
                if (IsKeyDown(cfg.FreeCamTiltRightKey)) { roll -= TiltSpeed * dt; changed = true; }
                if (IsKeyDown(cfg.FreeCamResetTiltKey) && roll != 0f) { roll = 0f; changed = true; }

                var cosP = (float)Math.Cos(pitch);
                var sinP = (float)Math.Sin(pitch);
                var sinY = (float)Math.Sin(yaw);
                var cosY = (float)Math.Cos(yaw);

                var nfx = sinY * cosP;
                var nfy = sinP;
                var nfz = cosY * cosP;

                var r0x = cosY;
                var r0z = -sinY;

                var u0x = nfy * r0z;
                var u0y = nfz * r0x - nfx * r0z;
                var u0z = -nfy * r0x;

                var cosR = (float)Math.Cos(roll);
                var sinR = (float)Math.Sin(roll);

                var nrx = r0x * cosR + u0x * sinR;
                var nry = u0y * sinR;
                var nrz = r0z * cosR + u0z * sinR;

                var nux = u0x * cosR - r0x * sinR;
                var nuy = u0y * cosR;
                var nuz = u0z * cosR - r0z * sinR;

                var move = (IsKeyDown(cfg.FreeCamFastKey) ? FastSpeed : BaseSpeed) * dt;

                if (IsKeyDown(cfg.FreeCamForwardKey)) { px += nfx * move; py += nfy * move; pz += nfz * move; changed = true; }
                if (IsKeyDown(cfg.FreeCamBackwardKey)) { px -= nfx * move; py -= nfy * move; pz -= nfz * move; changed = true; }
                if (IsKeyDown(cfg.FreeCamRightKey)) { px += nrx * move; py += nry * move; pz += nrz * move; changed = true; }
                if (IsKeyDown(cfg.FreeCamLeftKey)) { px -= nrx * move; py -= nry * move; pz -= nrz * move; changed = true; }
                if (IsKeyDown(cfg.FreeCamUpKey)) { py += move; changed = true; }
                if (IsKeyDown(cfg.FreeCamDownKey)) { py -= move; changed = true; }

                if (changed)
                {
                    var outMat = new byte[0x40];
                    Array.Copy(BitConverter.GetBytes(nrx), 0, outMat, 0, 4);
                    Array.Copy(BitConverter.GetBytes(nry), 0, outMat, 4, 4);
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
