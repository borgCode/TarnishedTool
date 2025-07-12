namespace SilkyRing.Memory.DLLShared
{
    public enum LogCommand
    {
        LogAiGoals = 0,
        LogSetEvent = 10,
        LogUniqueSetEvent = 11,
        ClearUniqueSetEvent = 12,
        LogApplySpeffect = 20,
        LogUniqueSpeffect = 21,
        ClearUniqueSpeffect = 22,
        MaxCount = 100
    }
}