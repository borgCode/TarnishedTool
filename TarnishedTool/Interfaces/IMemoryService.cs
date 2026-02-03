// 

using System;
using System.Diagnostics;
using System.Numerics;

namespace TarnishedTool.Interfaces;

public interface IMemoryService
{
    public bool IsAttached { get; }
    public Process? TargetProcess { get; }
    public nint BaseAddress { get; }
    public int ModuleMemorySize { get; }
    
    ulong ReadUInt64(nint addr);
    long ReadInt64(nint addr);
    string ReadString(nint addr, int maxLength = 32);
    byte[] ReadBytes(nint addr, int size);
    public nint FollowPointers(nint baseAddress, int[] offsets, bool readFinalPtr, bool derefBase = true);

    T[] ReadArray<T>(IntPtr addr, int count) where T : unmanaged;
    T Read<T>(IntPtr addr) where T : unmanaged;
    string HexDump(nint addr, int size);

    void Write<T>(IntPtr addr, T value) where T : unmanaged;
    void Write(IntPtr addr, bool value);
    void WriteString(nint addr, string value, int maxLength = 32);
    void WriteBytes(IntPtr addr, byte[] val);

    void SetBitValue(nint addr, int flagMask, bool setValue);
    bool IsBitSet(nint addr, int flagMask);

    void RunThread(nint address, uint timeout = uint.MaxValue);

    void AllocateAndExecute(byte[] shellcode);
    void AllocCodeCave();

    nint AllocateMem(uint size);
    void FreeMem(nint addr);
    
    void StartAutoAttach();
}