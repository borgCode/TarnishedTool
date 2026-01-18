// 

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;

namespace TarnishedTool.Services;

public class EventLogReader(MemoryService memoryService) : IEventLogReader, IDisposable
{
    private DispatcherTimer _timer;
    private int _readIndex;
    
    private IntPtr _writeIndexAddr;
    private IntPtr _bufferAddr;
    
    public event Action<List<EventLogEntry>> EntriesReceived;
    
    public void Start()
    {
        _readIndex = 0;
        _writeIndexAddr = CodeCaveOffsets.Base + CodeCaveOffsets.EventLogWriteIndex;
        _bufferAddr = CodeCaveOffsets.Base + CodeCaveOffsets.EventLogBuffer;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _timer.Tick += Poll;
        
        _timer.Start();
    }

    public void Stop() => _timer?.Stop();
    public void Dispose() => _timer?.Stop();
    
    private void Poll(object sender, EventArgs e)
    {
        var writeIndex = memoryService.ReadInt32(_writeIndexAddr);
        if (writeIndex == _readIndex) return;

        var entries = new List<EventLogEntry>();
        
        while (_readIndex != writeIndex)
        {
            var offset = _readIndex * 5;
            var eventId = memoryService.ReadUInt32(_bufferAddr + offset);
            var value = memoryService.ReadUInt8(_bufferAddr + offset + 4) != 0;
            entries.Add(new EventLogEntry(eventId, value));
            
            _readIndex = (_readIndex + 1) & 511;
        }

        EntriesReceived?.Invoke(entries);
    }
   
}