using BaseSource.ViewModels.Setting;

namespace BaseSource.Services.Services.Setting
{
    public interface IConfigSettingService
    {
        Task<ConfigSettingVm> GetSettingAsync();
        Task<KeyValuePair<bool, string>> UpdateAsync(ConfigSettingVm model);
    }
}
