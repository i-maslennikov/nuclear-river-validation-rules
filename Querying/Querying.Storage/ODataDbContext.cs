using System.Data.Entity;

namespace NuClear.Querying.Storage
{
    public sealed class ODataDbContext : DbContext
    {
        static ODataDbContext()
        {
            // disable DbContext type cache
            Database.SetInitializer(new NullDatabaseInitializer<ODataDbContext>());
        }
    }
}