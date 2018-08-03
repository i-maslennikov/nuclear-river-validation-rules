using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Storage;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public sealed class UpdateTableStatisticsActor : IActor
    {
        private readonly SqlConnection _sqlConnection;

        public UpdateTableStatisticsActor(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var updateStatisticsCommands = commands.OfType<UpdateTableStatisticsCommand>()
                                                   .ToList();

            if (!updateStatisticsCommands.Any())
            {
                return Array.Empty<IEvent>();
            }

            foreach (var command in updateStatisticsCommands)
            {
                try
                {
                    var database = _sqlConnection.GetDatabase();
                    var table = database.GetTable(command.Table);
                    table.UpdateStatistics(command.StatisticsTarget, command.StatisticsScanType);
                }
                catch (Exception ex)
                {
                    throw new DataException($"Error occurred while statistics updating for table {command.Table}", ex);
                }
            }

            return Array.Empty<IEvent>();
        }

        public sealed class UpdateTableStatisticsCommand : ICommand
        {
            public UpdateTableStatisticsCommand(TableName table,
                                                StatisticsTarget statisticsTarget,
                                                StatisticsScanType statisticsScanType)
            {
                Table = table;
                StatisticsTarget = statisticsTarget;
                StatisticsScanType = statisticsScanType;
            }

            public TableName Table { get; }
            public StatisticsTarget StatisticsTarget { get; }
            public StatisticsScanType StatisticsScanType { get; }
        }
    }
}
