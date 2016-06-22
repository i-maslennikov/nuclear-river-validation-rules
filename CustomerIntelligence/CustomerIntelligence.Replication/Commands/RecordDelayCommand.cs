using System;

using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class RecordDelayCommand : ICommand
    {
        public RecordDelayCommand(DateTime eventTime)
        {
            EventTime = eventTime;
        }

        public DateTime EventTime { get; }
    }
}
