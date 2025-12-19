// 

namespace SilkyRing.GameIds;

public static class EzState
{
    public class TalkCommand(int commandId, int[] @params)
    {
        public int CommandId { get; } = commandId;
        public int[] Params { get; } = @params;
    }

    public static class TalkCommands
    {
        public static readonly TalkCommand OpenKaleShop = new(22, [100500, 100524]);
        
        
        public static readonly TalkCommand OpenAttunement = new(28, [-1, -1]);
        public static readonly TalkCommand OpenChest = new(30, []);
        public static readonly TalkCommand OpenSell = new(46, [-1, -1]); //Needs player handle
        public static readonly TalkCommand OpenAllot = new(105, []); 
        public static readonly TalkCommand OpenPhysick = new(130, []);

        public static readonly TalkCommand AcquireGesture = new(131, [108]); // TODO call with all gesture ids
    }
}