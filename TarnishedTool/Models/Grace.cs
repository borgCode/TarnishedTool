namespace TarnishedTool.Models
{
    public class Grace
    {
        public bool IsDlc { get; set; }
        public string MainArea { get; set; }
        public string Name { get; set; }
        public long GraceEntityId { get; set; }
        public int FlagId { get; set; }

        public Grace Clone() => (Grace)MemberwiseClone();
    }
}