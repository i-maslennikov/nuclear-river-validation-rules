using System.Collections.Generic;

namespace NuClear.ValidationRules.WebApp.Model
{
    public class ValidationResult
    {
        public int Rule { get; set; }
        public string Template { get; set; }
        public IReadOnlyCollection<EntityReference> References { get; set; }
        public EntityReference MainReference { get; set; }
        public Level Result { get; set; }

        public class EntityReference
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
        }

        public enum Level
        {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
        }
    }
}