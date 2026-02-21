// 

using static TarnishedTool.Enums.GameVersion;

namespace TarnishedTool.Memory;

public static class OriginalBytesByPatch
{
    public static class AttackInfo
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
           Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_3_0 or Version1_3_1 or Version1_3_2 or Version1_4_0 or Version1_4_1 or Version1_5_0 or Version1_6_0
                or Version1_7_0 or Version1_8_0 or Version1_8_1
                or Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 => [0xF3, 0x44, 0x0F, 0x59, 0xD0],
            _ => [0xF3, 0x44, 0x0F, 0x59, 0xC8]
        };
    }

    public static class NoRunesFromEnemies
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
           Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1 or Version1_3_2 or Version1_4_0  or Version1_4_1 or Version1_5_0
                or Version1_6_0 =>
                [0x41, 0xFF, 0x91, 0xA8, 0x05, 0x00, 0x00],
            Version1_7_0 or Version1_8_1 or Version1_9_0 or Version1_9_1 or Version2_0_0 or Version2_0_1 =>
                [0x41, 0xFF, 0x91, 0xB8, 0x05, 0x00, 0x00],
            _ => [0x41, 0xFF, 0x91, 0xC8, 0x05, 0x00, 0x00]
        };
    }
    

    public static class InfinitePoise
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
          Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 => [0x4C, 0x8B, 0xC7, 0x41, 0x0F, 0xB6, 0xD6],
            _ => [0x4C, 0x8B, 0xC7, 0x40, 0x0F, 0xB6, 0xD5]
        };
    }

    public static class GetForceActIdx
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
            Version1_2_0 or Version1_2_1 or Version1_2_2 or Version1_2_3 or Version1_3_0 or Version1_3_1 or Version1_3_2 or Version1_4_0 or Version1_4_1 or Version1_5_0
                or Version1_6_0 =>
                [0x0F, 0xBE, 0x80, 0xB1, 0xE9, 0x00, 0x00],
            _ => [0x0F, 0xBE, 0x80, 0xC1, 0xE9, 0x00, 0x00]
        };
    }
}