using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;

namespace NuClear.ValidationRules.Replication.SystemRules.Validation
{
    // Если последнее сообщение в Kafka от AMS имеет timestamp, отличающийся от текущего времени более чем на epsilon, должна выводиться ошибка
    // "Данные из системы AMS слишком старые, результаты проверок недостоверны"
    public sealed class AmsMessagesShouldBeNew : ValidationResultAccessorBase
    {
        public AmsMessagesShouldBeNew(IQuery query) : base(query, MessageTypeCode.AmsMessagesShouldBeNew)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages = from fail in query.For<SystemStatus>()
                                             .Where(x => x.Id == Storage.Model.Facts.SystemStatus.SystemId.Ams)
                                             .Where(x => x.SystemIsDown)
                           select new Version.ValidationResult
                           {
                               MessageParams = new MessageParams().ToXDocument(),
                               PeriodStart = System.DateTime.MinValue,
                               PeriodEnd = System.DateTime.MaxValue,
                           };
            return messages;
        }
    }
}
