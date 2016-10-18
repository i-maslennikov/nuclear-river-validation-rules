namespace NuClear.ValidationRules.Storage.Model.FirmRules.Facts
{
    // todo: удалить эту сущность
    // можно объединить с opa, а пересчитывать их по изменениям в op и opa - кажется, так будет лучше.
    public class OrderPosition
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
    }
}