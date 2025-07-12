using System;
using System.Data;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace SilkyRing.Memory.DLLShared
{
    public class DllManager
    {
        private readonly MemoryIo _memoryIo;
        private readonly string _dllPath;
        private MemoryMappedFile _addrSharedMem;
        private MemoryMappedViewAccessor _viewAccessor;
        
        private MemoryMappedFile _logSharedMem;
        private MemoryMappedViewAccessor _logAccessor;
        private Thread _logReaderThread;
        private bool _isLogReading;
        public event EventHandler<string> LogReceived;
        
        private const int LogBufferSize = 1048576; // 1MB
        private const int WritePosOffset = LogBufferSize;
        private const int ReadPosOffset = LogBufferSize + sizeof(int);
        private const int CommandArrayOffset = ReadPosOffset + sizeof(int);

        
        public DllManager(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
            _dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "dll.dll");
            
        }
        private bool _isInjected;
        private bool _isAddrSharedMemInitialized;
        private bool _isLogSharedMemInitialized;

        public void EnsureInjectedDll()
        {
            if (_isInjected) return;
            if (!_isAddrSharedMemInitialized) _isAddrSharedMemInitialized = InitAddrSharedMem();
            if (!_isLogSharedMemInitialized) _isLogSharedMemInitialized = InitLogSharedMem();
            _isInjected = _memoryIo.InjectDll(_dllPath);
            
        }

        private bool InitAddrSharedMem()
        {
            
            _addrSharedMem = MemoryMappedFile.CreateOrOpen(
                "AddrSharedMem",
                sizeof(long),
                MemoryMappedFileAccess.ReadWrite);

            _viewAccessor = _addrSharedMem.CreateViewAccessor();
            _viewAccessor.Write(0, CodeCaveOffsets.Base.ToInt64() + (int)CodeCaveOffsets.LockedTarget.SavedPtr);
            return true;
        }

        private bool InitLogSharedMem()
        {
           
            _logSharedMem = MemoryMappedFile.CreateOrOpen(
                "LoggerSharedMem",
                LogBufferSize + (sizeof(int) * 2) + (sizeof(int) * (int)LogCommand.MaxCount), 
                MemoryMappedFileAccess.ReadWrite);
                
            _logAccessor = _logSharedMem.CreateViewAccessor();
            
            _logAccessor.Write(WritePosOffset, 0);
            _logAccessor.Write(ReadPosOffset, 0);
            
            return true;
        }
        
        public void StartLogReading()
        {
            if (!_isLogSharedMemInitialized || _isLogReading) return;
            
            _isLogReading = true;
            _logReaderThread = new Thread(LogReaderLoop) 
            { 
                IsBackground = true,
                Name = "Logger"
            };
            _logReaderThread.Start();
        }
        
        public void StopLogReading()
        {
            _isLogReading = false;
            _logReaderThread?.Join(100);
        }
        
        private void LogReaderLoop()
        {
            var buffer = new StringBuilder(4096);
            
            while (_isLogReading)
            {
                try
                {
                    int writePos = _logAccessor.ReadInt32(WritePosOffset);
                    int readPos = _logAccessor.ReadInt32(ReadPosOffset);
                    
                    if (readPos != writePos)
                    {
                        buffer.Clear();
                        
                        while (readPos != writePos)
                        {
                            byte b = _logAccessor.ReadByte(readPos);
                            buffer.Append((char)b);
                            readPos = (readPos + 1) % LogBufferSize;
                        }
                        
                        _logAccessor.Write(ReadPosOffset, readPos);
                        
                        string logs = buffer.ToString();
                        LogReceived?.Invoke(this, logs);
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }

                Thread.Sleep(16); 
            }
        }

        public void SetLogCommand(LogCommand command, bool isEnabled)
        {
            if (!_isLogSharedMemInitialized) return;
            int offset = CommandArrayOffset + ((int)command * sizeof(int));
            _logAccessor.Write(offset, isEnabled ? 1 : 0);
        }
    }
}