// 

using System;
using System.Runtime.InteropServices;

namespace TarnishedTool.Memory;

public readonly ref struct MemoryBlock(ReadOnlySpan<byte> data)
{
    private readonly ReadOnlySpan<byte> _data = data;

    public T Get<T>(int offset) where T : unmanaged 
        => MemoryMarshal.Read<T>(_data.Slice(offset));
}