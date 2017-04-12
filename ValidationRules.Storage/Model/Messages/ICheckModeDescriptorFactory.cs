namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public interface ICheckModeDescriptorFactory
    {
        ICheckModeDescriptor GetDescriptorFor(CheckMode checkMode);
    }
}