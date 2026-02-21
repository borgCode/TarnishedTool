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
    int sleepMax,
    int madnessCurrent,
    int madnessMax,
    int deathBlightCurrent,
    int deathBlightMax
   )
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
    
    public int MadnessCurrent { get; } = madnessCurrent;
    
    public int MadnessMax { get; } = madnessMax;
    
    public int DeathBlightCurrent { get; } = deathBlightCurrent;
    
    public int DeathBlightMax { get; } = deathBlightMax;
}