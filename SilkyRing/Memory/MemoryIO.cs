using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace SilkyRing.Memory
{
    public class MemoryIo : IDisposable
    {
        public Process TargetProcess;
        public IntPtr ProcessHandle = IntPtr.Zero;
        public IntPtr BaseAddress;

        private const int ProcessVmRead = 0x0010;
        private const int ProcessVmWrite = 0x0020;
        private const int ProcessVmOperation = 0x0008;
        public const int ProcessQueryInformation = 0x0400;

        private const string ProcessName = "eldenring";
        private bool _disposed;
        public bool IsAttached;
        
        private Timer _autoAttachTimer;
        
        public void StartAutoAttach()
        {
            _autoAttachTimer = new Timer(4000);
            _autoAttachTimer.Elapsed += (sender, e) => TryAttachToProcess();
            
            TryAttachToProcess();
    
            _autoAttachTimer.Start();
        }

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

        ~MemoryIo()
        {
            Dispose();
        }

        public bool ReadTest(IntPtr addr)
        {
            var array = new byte[1];
            var lpNumberOfBytesRead = 1;
            return Kernel32.ReadProcessMemory(ProcessHandle, addr, array, 1, ref lpNumberOfBytesRead) && lpNumberOfBytesRead == 1;
        }

        public void ReadTestFull(IntPtr addr)
        {
            Console.WriteLine($"Testing Address: 0x{addr.ToInt64():X}");

            bool available = ReadTest(addr);
            Console.WriteLine($"Availability: {available}");

            if (!available)
            {
                Console.WriteLine("Memory is not readable at this address.");
                return;
            }

            try
            {
                Console.WriteLine($"Int32: {ReadInt32(addr)}");
                Console.WriteLine($"Int64: {ReadInt64(addr)}");
                Console.WriteLine($"UInt8: {ReadUInt8(addr)}");
                Console.WriteLine($"UInt32: {ReadUInt32(addr)}");
                Console.WriteLine($"UInt64: {ReadUInt64(addr)}");
                Console.WriteLine($"Float: {ReadFloat(addr)}");
                Console.WriteLine($"Double: {ReadDouble(addr)}");
                Console.WriteLine($"String: {ReadString(addr)}");

                byte[] bytes = ReadBytes(addr, 16);
                Console.WriteLine("Bytes: " + BitConverter.ToString(bytes));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading memory: " + ex.Message);
            }
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

        public void AllocateAndExecute(byte[] shellcode)
        {
            IntPtr allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, IntPtr.Zero, (uint)shellcode.Length);
            
            if (allocatedMemory == IntPtr.Zero) return;
            
            WriteBytes(allocatedMemory, shellcode);
            bool executionSuccess = RunThreadAndWaitForCompletion(allocatedMemory);
            
            if (!executionSuccess) return;
            
            Kernel32.VirtualFreeEx(ProcessHandle, allocatedMemory, 0, 0x8000);
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

        public byte ReadUInt8(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 1);
            return bytes[0];
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

        public byte[] ReadBytes(IntPtr addr, int size)
        {
            var array = new byte[size];
            var lpNumberOfBytesRead = 1;
            Kernel32.ReadProcessMemory(ProcessHandle, addr, array, size, ref lpNumberOfBytesRead);
            return array;
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
                stringLength = bytes.Length - (bytes.Length % 2);
            }

            return Encoding.Unicode.GetString(bytes, 0, stringLength);
        }

        public void WriteInt32(IntPtr addr, int val)
        {
            WriteBytes(addr, BitConverter.GetBytes(val));
        }

        public void WriteFloat(IntPtr addr, float val)
        {
            WriteBytes(addr, BitConverter.GetBytes(val));
        }
        
        public void WriteDouble(IntPtr addr, double val)
        {
            WriteBytes(addr, BitConverter.GetBytes(val));
        }

        public void WriteUInt8(IntPtr addr, byte val)
        {
            var bytes = new byte[] { val };
            WriteBytes(addr, bytes);
        }

        public void WriteByte(IntPtr addr, int value)
        {
            Kernel32.WriteProcessMemory(ProcessHandle, addr, new byte[] { (byte)value }, 1, 0);
        }

        public void WriteBytes(IntPtr addr, byte[] val)
        {
            Kernel32.WriteProcessMemory(ProcessHandle, addr, val, val.Length, 0);
        }

        public void WriteString(IntPtr addr, string value, int maxLength = 32)
        {
            var bytes = new byte[maxLength];
            var stringBytes = Encoding.Unicode.GetBytes(value);
            Array.Copy(stringBytes, bytes, Math.Min(stringBytes.Length, maxLength));
            WriteBytes(addr, bytes);
        }

        
        internal IntPtr FollowPointers(IntPtr baseAddress, int[] offsets, bool readFinalPtr)
        {
            ulong ptr = ReadUInt64(baseAddress);
            
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                ptr = ReadUInt64((IntPtr)ptr + offsets[i]);
            }
            
            IntPtr finalAddress = (IntPtr)ptr + offsets[offsets.Length - 1];
            
            if (readFinalPtr) 
                return (IntPtr)ReadUInt64(finalAddress);
    
            
            return finalAddress;
        }

        public void SetBitValue(IntPtr addr, byte flagMask, bool setValue)
        {
            byte currentByte = ReadUInt8(addr);
            byte modifiedByte;
    
            if (setValue)
                modifiedByte = (byte)(currentByte | flagMask);
            else
                modifiedByte = (byte)(currentByte & ~flagMask);
            WriteUInt8(addr, modifiedByte);
        }
        
        public bool IsBitSet(IntPtr addr, byte flagMask)
        {
            byte currentByte = ReadUInt8(addr);
            
            return (currentByte & flagMask) != 0;
        }
        
        public void SetBit32(IntPtr addr, int bitPosition, bool setValue)
        {
            IntPtr wordAddr = IntPtr.Add(addr, (bitPosition / 32) * 4);
            
            int bitPos = bitPosition % 32;
            
            uint currentValue = ReadUInt32(wordAddr);
            
            uint bitMask = 1u << bitPos;
            
            uint newValue = setValue 
                ? currentValue | bitMask 
                : currentValue & ~bitMask;
            
            WriteInt32(wordAddr, (int)newValue);
        }
        
        public bool GetBit32(IntPtr addr, int bitPosition)
        {
            IntPtr wordAddr = IntPtr.Add(addr, (bitPosition / 32) * 4);
    
            int bitPos = bitPosition % 32;
    
            uint currentValue = ReadUInt32(wordAddr);
    
            uint bitMask = 1u << bitPos;
    
            return (currentValue & bitMask) != 0;
        }

        public bool IsGameLoaded()
        {
           return ReadUInt8((IntPtr)ReadUInt64(Offsets.MenuMan.Base) + Offsets.MenuMan.Offsets.IsLoaded) == 1;
        }
        
        public void AllocCodeCave()
        {
            IntPtr searchRangeStart = BaseAddress - 0x40000000;
            IntPtr searchRangeEnd = BaseAddress - 0x30000;
            uint codeCaveSize = 0x2000;
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
        //
        // public IntPtr GetModuleStart(IntPtr address)
        // {
        //     return Kernel32.QueryMemory(ProcessHandle, address).AllocationBase;
        // }
        public bool InjectDll(string dllPath)
        {
            if (!IsAttached || ProcessHandle == IntPtr.Zero) return false;


            if (!File.Exists(dllPath)) return false;

            try
            {
                string fullDllPath = Path.GetFullPath(dllPath);

                byte[] dllPathBytes = Encoding.Unicode.GetBytes(fullDllPath + "\0");
                IntPtr dllPathAddress = Kernel32.VirtualAllocEx(
                    ProcessHandle,
                    IntPtr.Zero,
                    (uint)dllPathBytes.Length);

                if (dllPathAddress == IntPtr.Zero) return false;
                WriteBytes(dllPathAddress, dllPathBytes);
                
                IntPtr loadLibraryAddr = GetProcAddress("kernel32.dll", "LoadLibraryW");
                
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to get LoadLibraryW address");
                    Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000); // MEM_RELEASE
                    return false;
                }

                IntPtr threadHandle = Kernel32.CreateRemoteThread(
                    ProcessHandle,
                    IntPtr.Zero,
                    0,
                    loadLibraryAddr,
                    dllPathAddress,
                    0,
                    IntPtr.Zero);

                if (threadHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to create remote thread");
                    Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000);
                    return false;
                }

                uint result = Kernel32.WaitForSingleObject(threadHandle, 5000);
                Kernel32.CloseHandle(threadHandle);

                Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000);

                return result == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error injecting DLL: {ex.Message}");
                return false;
            }
            
            
        }
        
        public IntPtr GetProcAddress(string moduleName, string procName)
        {
            IntPtr moduleHandle = Kernel32.GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
                return IntPtr.Zero;

            return Kernel32.GetProcAddress(moduleHandle, procName);
        }

    }
}
