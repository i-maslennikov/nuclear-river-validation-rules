using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ValidationRules.Replication.DatabaseComparison
{
    internal class Saver<T> where T : class
    {
        public void Save(EntityChanges<T> changes)
        {
            var name = typeof(T).FullName.Replace("NuClear.ValidationRules.Storage.Model.", "");
            var directory = new DirectoryInfo(Path.Combine(@"c:\dev\data", name));
            if (directory.Exists)
            {
                directory.Delete(true);
                directory = new DirectoryInfo(Path.Combine(@"c:\dev\data", name));
            }

            Write(directory, nameof(changes.DestOnly), changes.DestOnly.ToArray());
            Write(directory, nameof(changes.SourceOnly), changes.SourceOnly.ToArray());
            Write(directory, nameof(changes.DestChanged), changes.DestChanged.ToArray());
            Write(directory, nameof(changes.SourceChanged), changes.SourceChanged.ToArray());
        }

        private void Write(DirectoryInfo directory, string name, IReadOnlyCollection<T> changes)
        {
            if (!changes.Any())
            {
                return;
            }

            if (!directory.Exists)
            {
                directory.Create();
            }

            File.WriteAllLines(Path.Combine(directory.FullName, $"{name}.txt"), changes.Select(Serialize).ToArray());
        }

        private static string Serialize(T t)
            => Newtonsoft.Json.JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.None);
    }
}