using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Querying.Storage.Tests
{
    internal static class EdmModelExtensions
    {
        public static EntitySet FindEntitySet(this EdmModel model, string name)
        {
            return model.Container.EntitySets.FirstOrDefault(x => x.Name == name);
        }

        public static AssociationSet FindAssociationSet(this EdmModel model, string name)
        {
            return model.Container.AssociationSets.FirstOrDefault(x => x.Name == name);
        }

        public static AssociationType FindAssociationType(this EdmModel model, string name)
        {
            return model.AssociationTypes.FirstOrDefault(x => IsCompositeName(name) ? x.FullName == name : x.Name == name);
        }

        public static EntityType FindEntityType(this EdmModel model, string name)
        {
            return model.EntityTypes.FirstOrDefault(x => IsCompositeName(name) ? x.FullName == name : x.Name == name);
        }

        public static EdmProperty FindProperty(this EntityType entityType, string name)
        {
            return entityType.Properties.FirstOrDefault(x => IsCompositeName(name) ? x.Name == name : x.Name == name);
        }

        private static bool IsCompositeName(string name)
        {
            return name != null && name.Contains(".");
        }
    }
}