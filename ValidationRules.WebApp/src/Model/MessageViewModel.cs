using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Model
{
    public class MessageViewModel
    {
        private static readonly object[] FooBar = Enumerable.Repeat("...", 10).ToArray();

        private static readonly IDictionary<Result, string> Classes =
            new Dictionary<Result, string> { { Result.Info, "info" }, { Result.Warning, "warning" }, { Result.Error, "danger" } };

        private readonly MessageTemplate _message;
        private readonly Result _result;

        public MessageViewModel(Result result, MessageTemplate message, string orderLink)
        {
            _message = message;
            _result = result;
            OrderLink = orderLink;
        }

        public int MessageType
            => _message.MessageType;

        public Tuple<string, long, string> Order
            => _message.Order;

        public long OrderId
            => _message.Order.Item2;

        public string OrderLink { get; }

        public Result Result
            => _result;

        public string Header
            => string.Format(_message.Template, FooBar);

        public string Text
            => string.Format(_message.Template, _message.Arguments.ToArray());

        public string Class
            => Classes[_result];
    }
}