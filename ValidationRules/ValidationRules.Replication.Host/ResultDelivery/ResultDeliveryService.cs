using System;
using System.Linq;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Slack;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultDeliveryService
    {
        private readonly ITransportDecorator _transport;
        private readonly UserReadModel _userReadModel;
        private readonly ResultReadModel _resultReadModel;
        private readonly LocalizedMessageFactory _localizedMessageFactory;

        public ResultDeliveryService(SlackTransportDecorator transport, UserReadModel userReadModel, ResultReadModel resultReadModel, LocalizedMessageFactory localizedMessageFactory)
        {
            _transport = transport;
            _userReadModel = userReadModel;
            _resultReadModel = resultReadModel;
            _localizedMessageFactory = localizedMessageFactory;
        }

        public void Execute(DateTime iterationTime)
        {
            var currentIteration = _userReadModel.GetCurrentIteration(iterationTime);

            foreach (var user in currentIteration.GroupBy(x => x.UserAccount, x => Tuple.Create(x.OrderId, x.PeriodStart)))
            {
                var messages = _resultReadModel.GetResults(user)
                                               .Select(_localizedMessageFactory.Localize)
                                               .ToArray();

                _transport.SendMessage(user.Key, messages);
            }
        }
    }
}