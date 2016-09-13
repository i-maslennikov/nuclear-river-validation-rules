using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultDeliverySettingsAspect : ISettingsAspect
    {
        private readonly IntSetting _deleveryHour = ConfigFileSetting.Int.Optional("ResultDeliveryHour", 9);
        private readonly StringSetting _ermAddress = ConfigFileSetting.String.Required("ErmAddress");

        public int ResultDeliveryHour => _deleveryHour.Value;
        public string ErmAddress => _ermAddress.Value;
    }
}
