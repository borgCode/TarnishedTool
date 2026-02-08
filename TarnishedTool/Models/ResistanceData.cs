// 

namespace TarnishedTool.Models;

public class ResistanceData(
    int poisonCurrent,
    int poisonMax,
    int rotCurrent,
    int rotMax,
    int bleedCurrent,
    int bleedMax,
    int frostCurrent,
    int frostMax,
    int sleepCurrent,
    int sleepMax)
{
    public int PoisonCurrent { get; } = poisonCurrent;
    public int PoisonMax { get; } = poisonMax;
    public int RotCurrent { get; } = rotCurrent;
    public int RotMax { get; } = rotMax;
    public int BleedCurrent { get; } = bleedCurrent;
    public int BleedMax { get; } = bleedMax;
    public int FrostCurrent { get; } = frostCurrent;
    public int FrostMax { get; } = frostMax;
    public int SleepCurrent { get; } = sleepCurrent;
    public int SleepMax { get; } = sleepMax;
}