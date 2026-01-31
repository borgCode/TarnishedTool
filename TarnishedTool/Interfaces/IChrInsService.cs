// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IChrInsService
{
    List<ChrInsEntry> GetNearbyChrInsEntries();
    int GetChrIdByChrIns(IntPtr chrIns);
    uint GetNpcParamIdByChrIns(IntPtr chrIns);
    long GetHandleByChrIns(IntPtr chrIns);
    void SetSelected(nint chrIns, bool isSelected);
    Position GetChrInsPos(IntPtr chrIns);
    Vector3 GetChrInsLocalPos(IntPtr chrIns);
}