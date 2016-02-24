using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Dto
{
    public sealed class OrderValidationConfig : IDataTransferObject
    {
        public IReadOnlyCollection<Position> Positions { get; set; }

        public sealed class Position
        {
            public long Id { get; set; }
            public IReadOnlyCollection<PositionBinding> MasterPositions { get; set; }
            public IReadOnlyCollection<PositionBinding> DeniedPositions { get; set; }
        }

        public sealed class PositionBinding
        {
            public long Id { get; set; }
            public string BindingType { get; set; }
        }
    }
}
