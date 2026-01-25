// 

using System;

namespace TarnishedTool.Interfaces;

public interface IGameTickService
{
    public void Subscribe(Action callback);
    public void Unsubscribe(Action callback);
}