using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class ReplaceDataObjectCommand : IReplaceDataObjectCommand
    {
        public Type DataObjectType { get; }
        public object Dto { get; }

        public ReplaceDataObjectCommand(Type dataObjectType, object dto)
        {
            DataObjectType = dataObjectType;
            Dto = dto;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var a = obj as ReplaceDataObjectCommand;
            return a != null && Equals(a);
        }

        private bool Equals(ReplaceDataObjectCommand other)
        {
            return DataObjectType == other.DataObjectType && Dto.Equals(other.Dto);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DataObjectType.GetHashCode() * 397) ^ Dto.GetHashCode();
            }
        }
    }
}