using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace SilkyRing.Memory.DLLShared
{
    public class DllManager
    {
        private readonly MemoryIo _memoryIo;
        private readonly string _dllPath;
        private MemoryMappedFile _sharedMem;
        private MemoryMappedViewAccessor _viewAccessor;
        
        public DllManager(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
            _dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "dll.dll");
            
        }
        private bool _isInjected;
        private bool _isSharedMemInitialized;

        public void InjectDll()
        {
            if (_isInjected) return;
            if (!_isSharedMemInitialized) _isSharedMemInitialized = InitSharedMem();
            _isInjected = _memoryIo.InjectDll(_dllPath);
            
        }

        private bool InitSharedMem()
        {
            
            _sharedMem = MemoryMappedFile.CreateOrOpen(
                "SharedMem",
                sizeof(long),
                MemoryMappedFileAccess.ReadWrite);

            _viewAccessor = _sharedMem.CreateViewAccessor();
            _viewAccessor.Write(0, CodeCaveOffsets.Base.ToInt64() + (int)CodeCaveOffsets.LockedTarget.SavedPtr);
            return true;
        }
    }
}