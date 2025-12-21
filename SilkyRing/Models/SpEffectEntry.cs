// 

namespace SilkyRing.Models;

public class SpEffectEntry(int id, float timeLeft, float duration)
{
    public int Id { get; set; } = id;
    public float TimeLeft { get; set; } = timeLeft;
    public float Duration { get; set; } = duration;
}