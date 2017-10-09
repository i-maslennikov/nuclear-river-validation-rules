namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class SystemStatus
    {
        public long Id { get; set; }
        public bool SystemIsDown { get; set; }

        public static class SystemId
        {
            public static readonly long Ams = 1;
        }
    }
}
