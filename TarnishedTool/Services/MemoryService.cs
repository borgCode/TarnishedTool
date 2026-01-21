using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Timers;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;

namespace TarnishedTool.Services
{
    public class MemoryService : IMemoryService
    {
        public bool IsAttached { get; private set; }
        public Process? TargetProcess { get; private set; }
        public IntPtr ProcessHandle { get; private set; } = IntPtr.Zero;
        public IntPtr BaseAddress { get; private set; }
        public int ModuleMemorySize { get; private set; }

        private const int ProcessVmRead = 0x0010;
        private const int ProcessVmWrite = 0x0020;
        private const int ProcessVmOperation = 0x0008;
        private const int ProcessQueryInformation = 0x0400;
        private const int AttachCheckInterval = 2000; //MS

        private const string ProcessName = "eldenring";
        private bool _disposed;

        private Timer _autoAttachTimer;

        
        public byte ReadUInt8(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 1);
            return bytes[0];
        }

        public ushort ReadUInt16(nint addr)
        {
            var bytes = ReadBytes(addr, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }
        
        public short ReadInt16(nint addr)
        {
            var bytes = ReadBytes(addr, 2);
            return BitConverter.ToInt16(bytes, 0);
        }

        public uint ReadUInt32(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public ulong ReadUInt64(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 8);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public int ReadInt32(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public long ReadInt64(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public float ReadFloat(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public double ReadDouble(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public string ReadString(IntPtr addr, int maxLength = 32)
        {
            var bytes = ReadBytes(addr, maxLength * 2);

            int stringLength = 0;
            for (int i = 0; i < bytes.Length - 1; i += 2)
            {
                if (bytes[i] == 0 && bytes[i + 1] == 0)
                {
                    stringLength = i;
                    break;
                }
            }

            if (stringLength == 0)
            {
                stringLength = bytes.Length - bytes.Length % 2;
            }

            return Encoding.Unicode.GetString(bytes, 0, stringLength);
        }
        
        public Vector3 ReadVector3(IntPtr address)
        {
            byte[] coordBytes = ReadBytes(address, 12);
            return new Vector3(
                BitConverter.ToSingle(coordBytes, 0),
                BitConverter.ToSingle(coordBytes, 4),
                BitConverter.ToSingle(coordBytes, 8)
            );
        }

        
        public byte[] ReadBytes(IntPtr addr, int size)
        {
            var array = new byte[size];
            var lpNumberOfBytesRead = 1;
            Kernel32.ReadProcessMemory(ProcessHandle, addr, array, size, ref lpNumberOfBytesRead);
            return array;
        }

        public void WriteUInt8(IntPtr addr, int val)
        {
            var bytes = new[] { (byte)val };
            WriteBytes(addr, bytes);
        }

        public void WriteInt16(IntPtr addr, short val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteUInt16(IntPtr addr, ushort val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteInt32(IntPtr addr, int val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteUInt32(IntPtr addr, uint val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteInt64(nint addr, long val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteFloat(IntPtr addr, float val) => WriteBytes(addr, BitConverter.GetBytes(val));
        public void WriteDouble(IntPtr addr, double val) => WriteBytes(addr, BitConverter.GetBytes(val));


        public void WriteString(IntPtr addr, string value, int maxLength = 32)
        {
            var bytes = new byte[maxLength];
            var stringBytes = Encoding.Unicode.GetBytes(value);
            Array.Copy(stringBytes, bytes, Math.Min(stringBytes.Length, maxLength));
            WriteBytes(addr, bytes);
        }
        
        public void WriteVector3(IntPtr address, Vector3 value)
        {
            byte[] coordBytes = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(value.X), 0, coordBytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(value.Y), 0, coordBytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(value.Z), 0, coordBytes, 8, 4);
            WriteBytes(address, coordBytes);
        }

        public void WriteBytes(IntPtr addr, byte[] val)
        {
            Kernel32.WriteProcessMemory(ProcessHandle, addr, val, val.Length, 0);
        }

        public void SetBitValue(IntPtr addr, int flagMask, bool setValue)
        {
            byte currentByte = ReadUInt8(addr);
            byte modifiedByte;

            if (setValue)
                modifiedByte = (byte)(currentByte | flagMask);
            else
                modifiedByte = (byte)(currentByte & ~flagMask);
            WriteUInt8(addr, modifiedByte);
        }

        public bool IsBitSet(IntPtr addr, int flagMask)
        {
            byte currentByte = ReadUInt8(addr);

            return (currentByte & flagMask) != 0;
        }

        public uint RunThread(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
            var ret = Kernel32.WaitForSingleObject(thread, timeout);
            Kernel32.CloseHandle(thread);
            return ret;
        }

        public bool RunThreadAndWaitForCompletion(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);

            if (thread == IntPtr.Zero)
            {
                return false;
            }

            uint waitResult = Kernel32.WaitForSingleObject(thread, timeout);
            Kernel32.CloseHandle(thread);

            return waitResult == 0;
        }

        public IntPtr FollowPointers(IntPtr baseAddress, int[] offsets, bool readFinalPtr, bool derefBase = true)
        {
            ulong ptr = derefBase ? ReadUInt64(baseAddress) : (ulong)baseAddress;

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                ptr = ReadUInt64((IntPtr)ptr + offsets[i]);
            }

            IntPtr finalAddress = (IntPtr)ptr + offsets[offsets.Length - 1];

            if (readFinalPtr)
                return (IntPtr)ReadUInt64(finalAddress);

            return finalAddress;
        }

        public void AllocateAndExecute(byte[] shellcode)
        {
            IntPtr allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, IntPtr.Zero, (uint)shellcode.Length);

            if (allocatedMemory == IntPtr.Zero) return;

            WriteBytes(allocatedMemory, shellcode);
            bool executionSuccess = RunThreadAndWaitForCompletion(allocatedMemory);

            if (!executionSuccess) return;

            Kernel32.VirtualFreeEx(ProcessHandle, allocatedMemory, 0, 0x8000);
        }

        public void AllocCodeCave()
        {
            IntPtr searchRangeStart = BaseAddress - 0x40000000;
            IntPtr searchRangeEnd = BaseAddress - 0x30000;
            uint codeCaveSize = 0x5000;
            IntPtr allocatedMemory;

            for (IntPtr addr = searchRangeEnd; addr.ToInt64() > searchRangeStart.ToInt64(); addr -= 0x10000)
            {
                allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, addr, codeCaveSize);

                if (allocatedMemory != IntPtr.Zero)
                {
                    CodeCaveOffsets.Base = allocatedMemory;
                    break;
                }
            }
        }

        public IntPtr GetProcAddress(string moduleName, string procName)
        {
            IntPtr moduleHandle = Kernel32.GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
                return IntPtr.Zero;

            return Kernel32.GetProcAddress(moduleHandle, procName);
        }

        public void StartAutoAttach()
        {
            _autoAttachTimer = new Timer(AttachCheckInterval);
            _autoAttachTimer.Elapsed += (sender, e) => TryAttachToProcess();

            TryAttachToProcess();

            _autoAttachTimer.Start();
        }

        public void StopAutoAttach() => _autoAttachTimer.Stop();

        private void TryAttachToProcess()
        {
            if (ProcessHandle != IntPtr.Zero)
            {
                if (TargetProcess == null || TargetProcess.HasExited)
                {
                    Kernel32.CloseHandle(ProcessHandle);
                    ProcessHandle = IntPtr.Zero;
                    TargetProcess = null;
                    IsAttached = false;
                }

                return;
            }

            var processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length > 0 && !processes[0].HasExited)
            {
                TargetProcess = processes[0];
                ProcessHandle = Kernel32.OpenProcess(
                    ProcessVmRead | ProcessVmWrite | ProcessVmOperation | ProcessQueryInformation,
                    false,
                    TargetProcess.Id);

                if (ProcessHandle == IntPtr.Zero)
                {
                    TargetProcess = null;
                    IsAttached = false;
                }
                else
                {
                    if (TargetProcess.MainModule != null)
                    {
                        BaseAddress = TargetProcess.MainModule.BaseAddress;
                        ModuleMemorySize = TargetProcess.MainModule.ModuleMemorySize;
                    }

                    IsAttached = true;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_autoAttachTimer != null)
                {
                    _autoAttachTimer.Stop();
                    _autoAttachTimer.Dispose();
                    _autoAttachTimer = null;
                }

                if (ProcessHandle != IntPtr.Zero)
                {
                    Kernel32.CloseHandle(ProcessHandle);
                    ProcessHandle = IntPtr.Zero;
                    TargetProcess = null;
                    IsAttached = false;
                }

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        ~MemoryService()
        {
            Dispose();
        }
    }
}
