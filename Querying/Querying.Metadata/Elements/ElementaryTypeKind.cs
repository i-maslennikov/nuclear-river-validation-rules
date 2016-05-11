namespace NuClear.Querying.Metadata.Elements
{
    public enum ElementaryTypeKind
    {
        #region Integers

        Byte,
        Int16,
        Int32,
        Int64,

        #endregion

        #region Floats

        Single,
        Double,
        Decimal,

        #endregion

        Boolean,

        String,

        DateTimeOffset,
    }
}