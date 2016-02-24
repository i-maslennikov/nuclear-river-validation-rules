namespace NuClear.River.Common.Metadata.Elements
{
    public enum EntityRelationCardinality : byte
    {
        /// <summary>
        /// Specifies the relation that must direct to an entity.
        /// </summary>
        One,

        /// <summary>
        /// Specifies the relation that may direct to an entity.
        /// </summary>
        OptionalOne,

        /// <summary>
        /// Specifies the relation that must direct to at least an entity.
        /// </summary>
        Many,
    }
}