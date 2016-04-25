namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public abstract class ActivityReference
    {
        public long ActivityId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }
    }
}