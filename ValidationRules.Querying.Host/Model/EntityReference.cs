namespace NuClear.ValidationRules.Querying.Host.Model
{
    public class EntityReference
    {
        public EntityReference(string type, long id, string name)
        {
            Type = type;
            Id = id;
            Name = name;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
    }
}