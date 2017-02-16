using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public class MessageParams
    {
        private static readonly IReadOnlyDictionary<string, object> Empty = new Dictionary<string, object>();

        public MessageParams(params Reference[] children)
            : this(Empty, children)
        {
        }

        public MessageParams(IReadOnlyDictionary<string, object> extraParameters, params Reference[] children)
        {
            ExtraParameters = extraParameters;
            Children = children;
        }

        public IReadOnlyDictionary<string, object> ExtraParameters { get; }
        public IReadOnlyCollection<Reference> Children { get; }
    }
}