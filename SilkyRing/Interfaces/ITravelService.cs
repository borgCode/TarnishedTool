// 

using SilkyRing.Models;

namespace SilkyRing.Interfaces;

public interface ITravelService
{
    void Warp(Grace grace);
    void UnlockGrace(Grace grace);
}