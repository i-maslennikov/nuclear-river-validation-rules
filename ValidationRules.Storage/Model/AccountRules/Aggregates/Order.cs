﻿using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long DestProjectId { get; set; }
        public long? AccountId { get; set; }
        public bool IsFreeOfCharge { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }

        public sealed class Lock
        {
            public long OrderId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        public sealed class DebtPermission
        {
            public long OrderId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
