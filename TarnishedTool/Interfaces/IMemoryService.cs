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
    void Dispose();

    byte ReadUInt8(nint addr);
    ushort ReadUInt16(nint addr);
    uint ReadUInt32(nint addr);
    ulong ReadUInt64(nint addr);
    int ReadInt32(nint addr);
    long ReadInt64(nint addr);
    float ReadFloat(nint addr);
    double ReadDouble(nint addr);
    string ReadString(nint addr, int maxLength = 32);
    Vector3 ReadVector3(IntPtr address);
    byte[] ReadBytes(nint addr, int size);

    void WriteUInt8(nint addr, int val);
    void WriteInt16(nint addr, short val);
    void WriteInt32(nint addr, int val);
    void WriteUInt32(IntPtr addr, uint val);
    void WriteInt64(nint addr, long val);
    void WriteFloat(nint addr, float val);
    void WriteDouble(nint addr, double val);
    void WriteString(nint addr, string value, int maxLength = 32);
    void WriteVector3(IntPtr address, Vector3 value);
    void WriteBytes(IntPtr addr, byte[] val);

    void SetBitValue(nint addr, int flagMask, bool setValue);
    bool IsBitSet(nint addr, int flagMask);

    uint RunThread(nint address, uint timeout = uint.MaxValue);
    bool RunThreadAndWaitForCompletion(nint address, uint timeout = uint.MaxValue);

    public nint FollowPointers(nint baseAddress, int[] offsets, bool readFinalPtr, bool derefBase = true);
    void AllocateAndExecute(byte[] shellcode);
    void AllocCodeCave();

    public IntPtr GetProcAddress(string moduleName, string procName);

    void StartAutoAttach();
    void StopAutoAttach();
}