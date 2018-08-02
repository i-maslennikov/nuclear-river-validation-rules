using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var command = commands.OfType<UpdateTableStatisticsCommand>().SingleOrDefault();
            if (command == null)
            {
                return Array.Empty<IEvent>();
            }

            try
            {
                var database = _sqlConnection.GetDatabase();
                var table = database.GetTable(command.Table);
                table.UpdateStatistics(command.StatisticsTarget, command.StatisticsScanType);

                return Array.Empty<IEvent>();
            }
            catch (Exception ex)
            {
                throw new DataException($"Error occured while statistics updating for table {command.Table}", ex);
            }
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
