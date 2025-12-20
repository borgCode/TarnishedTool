// 

namespace SilkyRing.Models;

public class SpEffectEntry(int id, float timeLeft, float duration)
{
    public uint Id { get; set; }
    public float TimeLeft { get; set; }
    public float Duration { get; set; }
}