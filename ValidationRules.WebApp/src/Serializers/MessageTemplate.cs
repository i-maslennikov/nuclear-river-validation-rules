using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public sealed class MessageTemplate : IEquatable<MessageTemplate>
    {
        public MessageTemplate(Tuple<string, long, string> order, string template, params object[] arguments)
        {
            Order = order;
            Template = template;
            Arguments = arguments;
        }

        public Tuple<string, long, string> Order { get; }
        public string Template { get; }
        public IReadOnlyCollection<object> Arguments { get; }
        public int MessageType { get; set; }

        public bool Equals(MessageTemplate other)
        {
            return Equals(this.Order, other.Order)
                   && string.Equals(this.Template, other.Template)
                   && EachEquals(this.Arguments, other.Arguments);
        }

        public override int GetHashCode()
        {
            return Order.GetHashCode() ^ Template.GetHashCode();
        }

        private bool EachEquals(IReadOnlyCollection<object> arguments, IReadOnlyCollection<object> readOnlyCollection)
        {
            return arguments.Count == readOnlyCollection.Count
                   && arguments.Zip(readOnlyCollection, Equals).Aggregate(true, (b, o) => b && o);
        }
    }
}
