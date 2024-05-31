using VectoTask.API.Models;

namespace VectoTask.API.Services.Interfaces
{
    public interface IPluginConfigurationManager : IPluginManager<RepositoryResponse<List<PluginConfig>>, PluginConfig>
    {
        Task<RepositoryResponse<List<PluginConfig>>> GetPluginsConfigsAsync();

        Task<RepositoryResponse<List<PluginConfig>>> SavePluginAsync(List<PluginConfig> pluginConfigs);
    }
}
