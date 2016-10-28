using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Model
{
    public sealed class ProjectViewModel
    {
        private readonly Result _result;
        private readonly MessageViewModel[] _messages;
        private readonly LinkFactory _linkFactory;

        private static readonly IDictionary<Result, string> Classes =
            new Dictionary<Result, string> { { Result.Info, "panel-info" }, { Result.Warning, "panel-warning" }, { Result.Error, "panel-danger" } };

        public ProjectViewModel(MessageViewModel[] messages, LinkFactory linkFactory)
        {
            _messages = messages;
            _result = messages.Max(x => x.Result);
            _linkFactory = linkFactory;
        }

        public string Title => _messages.First().Header;

        public string PanelId => $"rule_{_messages.First().MessageType}";

        public string Class => GetClass(_result);

        public IReadOnlyCollection<MessageViewModel> Messages => _messages;

        private static string GetClass(Result result)
        {
            string cls;
            if (!Classes.TryGetValue(result, out cls))
            {
                throw new ArgumentException($"Not supported: {result}", nameof(result));
            }

            return cls;
        }

    }
}