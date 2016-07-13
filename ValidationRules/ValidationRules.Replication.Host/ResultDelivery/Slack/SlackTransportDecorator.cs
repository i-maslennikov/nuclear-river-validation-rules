using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlackAPI;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Slack
{
    public sealed class SlackTransportDecorator : ITransportDecorator
    {
        private readonly SlackTaskClient _client;
        private readonly Task<LoginResponse> _connectionTask;

        public SlackTransportDecorator()
        {
            var apiToken = Environment.GetEnvironmentVariable("ORDER_VALIDATION_SLACK_TOKEN");
            _client = new SlackTaskClient(apiToken);
            _connectionTask = _client.ConnectAsync();
        }

        public IReadOnlyCollection<string> GetSubscribedUsers()
        {
            if (!_connectionTask.IsCompleted)
                _connectionTask.Wait();
            if (_connectionTask.IsFaulted)
                throw _connectionTask.Exception;

            return _client.Users.Select(x => x.profile.email.Split('@').First()).ToArray();
            return new[] { "a.rechkalov" };
        }

        public void SendMessage(string user, IReadOnlyCollection<LocalizedMessage> messages)
        {
            user = "a.rechkalov"; // fixme!
            if (!_connectionTask.IsCompleted)
                _connectionTask.Wait();
            if (_connectionTask.IsFaulted)
                throw _connectionTask.Exception;

            var slackUser = _client.Users.First(x => x.profile.email.StartsWith(user));
            var slackChannel = _client.JoinDirectMessageChannelAsync(slackUser.id).Result;
            _client.PostMessageAsync(slackChannel.channel.id, "Результаты проверки заказов", attachments: messages.GroupBy(x => x.Header, x => x).Select(MakeAttachment).ToArray()).Wait();
        }

        private Attachment MakeAttachment(IGrouping<string, LocalizedMessage> message)
        {
            var sortedByResult = message.OrderByDescending(x => x.Result);
            var lines = new List<string> { message.Key };
            lines.AddRange(sortedByResult.Select(item => $"{GetIcon(item.Result)} {item.Message}"));

            return new Attachment { text = string.Join("\n", lines), color = GetColor(sortedByResult.First().Result) };
        }

        private string GetColor(Result result)
        {
            switch (result)
            {
                case Result.Info:
                    return "good";
                case Result.Warning:
                    return "warning";
                case Result.Error:
                    return "danger";
                default:
                    return string.Empty;
            }
        }

        private string GetIcon(Result result)
        {
            switch (result)
            {
                case Result.Info:
                    return ":information_source:";
                case Result.Warning:
                    return ":warning:";
                case Result.Error:
                    return ":exclamation:";
                default:
                    return string.Empty;
            }
        }
    }
}