using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultDeliverySettingsAspect : ISettingsAspect
    {
        private readonly IntSetting _deleveryHour = ConfigFileSetting.Int.Optional("ResultDeliveryHour", 9);
        private readonly StringSetting _ermProduction = ConfigFileSetting.String.Required("ErmProduction");

        public int ResultDeliveryHour => _deleveryHour.Value;
        public string ErmProduction => _ermProduction.Value;
    }
}
