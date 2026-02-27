// 

using System.IO;

namespace TarnishedTool.GameIds;

public static class Emevd
{
    public class EmevdCommand(int groupId, int commandId, params object[] args)
    {
        public int GroupId { get; } = groupId;
        public int CommandId { get; } = commandId;
        public byte[] ParamData { get; } = Pack(args);

        private static byte[] Pack(object[] args)
        {
            if (args.Length == 0) return [];

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            int offset = 0;

            foreach (var arg in args)
            {
                int alignment = arg is sbyte or byte ? 1 : arg is short or ushort ? 2 : 4;
                int padding = (alignment - (offset % alignment)) % alignment;
                offset += padding + alignment;

                for (int i = 0; i < padding; i++) bw.Write((byte)0);

                switch (arg)
                {
                    case sbyte v: bw.Write(v); break;
                    case byte v: bw.Write(v); break;
                    case short v: bw.Write(v); break;
                    case ushort v: bw.Write(v); break;
                    case int v: bw.Write(v); break;
                    case uint v: bw.Write(v); break;
                    case float v: bw.Write(v); break;
                }
            }

            return ms.ToArray();
        }
    }

    public static class EmevdCommands
    {
        public static readonly EmevdCommand Rest = new(2004, 47);
        public static readonly EmevdCommand ReloadArea = new(2003, 70, (byte)1);

        public static readonly EmevdCommand SetMorning = new(
            2001,
            4,
            (byte)6, (byte)0, (byte)0, (byte)0, (byte)0, (byte)1, (float)0.75, (float)2.0, (float)0
        );

        public static readonly EmevdCommand SetNoon = new(
            2001,
            4,
            (byte)12, (byte)0, (byte)0, (byte)0, (byte)0, (byte)1, (float)0.75, (float)2.0, (float)0
        );

        public static readonly EmevdCommand SetDusk = new(
            2001,
            4,
            (byte)18, (byte)0, (byte)0, (byte)0, (byte)0, (byte)1, (float)0.75, (float)2.0, (float)0
            
        public static readonly EmevdCommand SetNight = new(
            2001,
            4,
            (byte)20, (byte)0, (byte)0, (byte)0, (byte)0, (byte)1, (float)0.75, (float)2.0, (float)0
        );

        public static EmevdCommand SetWeather(sbyte type) => new(
            2003,
            68,
            type,
            (float)-1, (byte)1
        );

        public static EmevdCommand ForcePlaybackAnimation(uint entityId, int animationId) => new(
            2003,
            18,
            entityId,
            animationId,
            (byte)0, // shouldLoop
            (byte)0, // shouldWaitForCompletion
            (byte)0 // ignoreWaitForTransition
        );

        public static EmevdCommand ResetCharacterPosition(uint entityId) => new(2004, 81, entityId);
        
        public static EmevdCommand AwardItemsIncludingClients(int itemId) => new(2003, 36, itemId);
    }
}
