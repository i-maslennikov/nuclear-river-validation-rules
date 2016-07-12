using System;
using System.Linq;

namespace NuClear.ValidationRules.Replication.Host.Temp
{
    public sealed class SlackResultDeliveryService : IResultDeliveryService
    {
        private readonly SlackDecorator _slackService;
        private readonly UserReadModel _userReadModel;
        private readonly ResultReadModel _resultReadModel;
        private readonly MessageSerializer _messageSerializer;

        public SlackResultDeliveryService(SlackDecorator slackService, UserReadModel userReadModel, ResultReadModel resultReadModel, MessageSerializer messageSerializer)
        {
            _slackService = slackService;
            _userReadModel = userReadModel;
            _resultReadModel = resultReadModel;
            _messageSerializer = messageSerializer;
        }

        public void DoIt()
        {
            var senderTime = DateTime.UtcNow;

            var currentIteration = _userReadModel.GetCurrentIteration(senderTime);

            foreach (var user in currentIteration.GroupBy(x => x.UserAccount, x => Tuple.Create(x.OrderId, x.PeriodStart)))
            {
                var checkResult = _resultReadModel.GetResults(user);
                foreach (var message in checkResult.GroupBy(x => x.ReferenceId)
                    .Select(x => $"Заказ {x.Key}\n" + string.Join("\n", x.Select(_messageSerializer.Serialize))))
                {
                    _slackService.SendMessage(user.Key, message);
                }
            }
        }
    }
}