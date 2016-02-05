using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Bulk.Metadata;
using NuClear.Replication.Bulk.API.Factories;
using NuClear.Replication.Bulk.API.Storage;

namespace NuClear.Replication.Bulk.API
{
    public sealed class BulkReplicationRunner
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly ViewRemover _viewRemover;

        public BulkReplicationRunner(IMetadataProvider metadataProvider, DataConnectionFactory dataConnectionFactory, ViewRemover viewRemover)
        {
            _metadataProvider = metadataProvider;
            _dataConnectionFactory = dataConnectionFactory;
            _viewRemover = viewRemover;
        }

        public void Run(string mode)
        {
            BulkReplicationMetadataElement bulkReplicationMetadata;
            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<BulkReplicationMetadataKindIdentity>(mode).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(id.Id, out bulkReplicationMetadata))
            {
                throw new NotSupportedException("Bulk replication metadata cannot be found");
            }

            var storageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.To);
            var context = bulkReplicationMetadata.Elements.Single();
            using (_viewRemover.TemporaryRemoveViews(storageDescriptor.ConnectionStringName, bulkReplicationMetadata.EssentialViews))
            {
                Parallel.ForEach(context.Elements,
                                 element =>
                                 {
                                     using (var bulkReplicatorFactory = CreateReplicatorFactory(bulkReplicationMetadata))
                                     {
                                         var sw = Stopwatch.StartNew();
                                         var replicators = bulkReplicatorFactory.Create(element);
                                         foreach (var replicator in replicators)
                                         {
                                             replicator.Replicate();
                                         }

                                         sw.Stop();
                                         Console.WriteLine($"{element.Identity.Id}: {sw.Elapsed.TotalSeconds} seconds");
                                     }
                                 });
            }
        }

        private IBulkReplicatorFactory CreateReplicatorFactory(BulkReplicationMetadataElement bulkReplicationMetadata)
        {
            var sourceStorageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.From);
            var targetStorageDescriptor = bulkReplicationMetadata.Features.OfType<StorageDescriptorFeature>().Single(x => x.Direction == ReplicationDirection.To);
            return new RoutingBulkReplicatorFactory(_dataConnectionFactory.CreateConnection(sourceStorageDescriptor), _dataConnectionFactory.CreateConnection(targetStorageDescriptor));
        }
    }
}