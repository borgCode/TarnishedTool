// 

namespace TarnishedTool.GameIds;

public static class EzState
{
    public const int TypeFloat = 1;
    public const int TypeInt = 2;
    public const int TypeObject = 3;

    public class TalkCommand(int commandId, int[] @params)
    {
        public int CommandId { get; } = commandId;
        public int[] Params { get; } = @params;
    }

    public class EnvQueryParam
    {
        public object Value { get; }
        public int TypeTag { get; }

        private EnvQueryParam(object value, int typeTag)
        {
            Value = value;
            TypeTag = typeTag;
        }

        public static EnvQueryParam Int(int value) => new(value, TypeInt);
        public static EnvQueryParam Float(float value) => new(value, TypeFloat);

        public static implicit operator EnvQueryParam(int value) => Int(value);
        public static implicit operator EnvQueryParam(float value) => Float(value);
    }

    public class EnvQueryResult
    {
        public int TypeTag { get; set; }
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public long PtrValue { get; set; }

        public bool IsInt => TypeTag == TypeInt;
        public bool IsFloat => TypeTag == TypeFloat;
        public bool IsObject => TypeTag == TypeObject;
    }

    public static class TalkCommands
    {
        public static TalkCommand OpenDialog(int type, int textId, int result, int style, int unk) =>
            new(17, [type, textId, result, style, unk]);

        public static readonly TalkCommand OpenUpgrade = new(24, [1]);
        public static readonly TalkCommand LevelUp = new(31, []);
        public static readonly TalkCommand OpenAttunement = new(28, [-1, -1]);
        public static readonly TalkCommand OpenChest = new(30, []);
        public static readonly TalkCommand OpenSell = new(46, [-1, -1]);
        public static readonly TalkCommand OpenAow = new(48, []);

        public static TalkCommand PlayerInventoryChange(int itemType, int itemId, int quantityChange) =>
            new(52, [itemType, itemId, quantityChange]);

        public static TalkCommand ReplaceTool(int itemIdToReplace, int newItemId, int newItemQuantity) =>
            new(59, [itemIdToReplace, newItemId, newItemQuantity]);

        public static readonly TalkCommand OpenAllot = new(105, []);
        public static TalkCommand EstusAllocationUpdate(int newFlaskCount) => new(108, [newFlaskCount, 0]);
        public static TalkCommand UpgradeFlask(int newLevel) => new(109, [newLevel]);
        public static readonly TalkCommand Rebirth = new(113, []);
        public static readonly TalkCommand OpenPhysick = new(130, []);
        public static readonly TalkCommand OpenMirror = new(81, []);

        public static TalkCommand AcquireGesture(int gestureId) => new(131, [gestureId]);

        public static readonly TalkCommand OpenGreatRunes = new(137, []);
        public static readonly TalkCommand OpenAlterGarments = new(142, [111000, 111399]);

        public static readonly TalkCommand[] UpgradeMenuFlags =
        [
            new(49, [6001, 232]),
            new(49, [6001, 233]),
            new(49, [6001, 234]),
            new(49, [6001, 235]),
        ];
        
        public static TalkCommand FadeOutAndPassTime(bool resetWorld) =>
            new(117, [
                resetWorld ? 1 : 0,  // resetWorld
                1,                    // resetMainCharacter
                1,                    // resetMagicCharges
                1,                    // restoreEstus
                0,                    // addHours
                0,                    // addMinutes
                0,                    // addSeconds
                0,                    // blackScreenTime
                0,                    // clockStartupDelayS
                0,                    // clockMoveTimeS
                0,                    // clockFinishDelayS
                0,                    // fadeOutTime
                0                     // fadeInTime
            ]);
    }

    public static class EnvQueries
    {
        public const int DoesPlayerHaveItem = 16;
        public const int ComparePlayerInventoryNumber = 47;
        public const int GetEstusAllocation = 109;
        public const int GetFlaskLevel = 110;
    }
}
