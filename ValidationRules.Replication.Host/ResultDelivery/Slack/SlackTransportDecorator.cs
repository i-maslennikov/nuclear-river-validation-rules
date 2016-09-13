using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NuClear.Tracing.API;

using SlackAPI;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Slack
{
    public sealed class SlackTransportDecorator : ITransportDecorator
    {
        private readonly ITracer _tracer;
        private readonly SlackTaskClient _client;
        private readonly Task<LoginResponse> _connectionTask;

        public SlackTransportDecorator(ITracer tracer)
        {
            _tracer = tracer;
            var apiToken = Environment.GetEnvironmentVariable("ORDER_VALIDATION_SLACK_TOKEN");
            _client = new SlackTaskClient(apiToken);
            _connectionTask = _client.ConnectAsync();
        }

        public IReadOnlyCollection<string> GetSubscribedUsers()
        {
            EnsureConnected();
            return _client.Users.Select(x => x.profile.email.Split('@').First()).ToArray();
        }

        public void SendMessage(string user, IReadOnlyCollection<LocalizedMessage> messages)
        {
            EnsureConnected();

            var slackUser = _client.Users.FirstOrDefault(x => x.profile?.email != null && x.profile.email.StartsWith(user));
            if (slackUser == null)
            {
                _tracer.Warn($"Пользователь {user} не найден, сообщения не отправлены");
                return;
            }

            var slackChannel = _client.JoinDirectMessageChannelAsync(slackUser.id).Result;
            if (messages.Any())
            {
                var message = $"Результаты проверки заказов ({user})";
                _client.PostMessageAsync(slackChannel.channel.id, message, attachments: messages.GroupBy(x => x.Header, x => x).Select(MakeAttachment).ToArray()).Wait();
            }
            else
            {
                var message = $"Проверка заказов ошибок не выявила ({user})";
                _client.PostMessageAsync(slackChannel.channel.id, message).Wait();
            }
        }

        private void EnsureConnected()
        {
            if (!_connectionTask.IsCompleted)
            {
                _connectionTask.Wait();
            }

            if (_connectionTask.IsFaulted)
            {
                throw new Exception("Can not connect to slack", _connectionTask.Exception);
            }
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