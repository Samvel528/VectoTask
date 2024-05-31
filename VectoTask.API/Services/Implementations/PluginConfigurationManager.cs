using Newtonsoft.Json;
using VectoTask.API.Constants;
using VectoTask.API.Models;
using VectoTask.API.Services.Interfaces;

namespace VectoTask.API.Services.Implementations
{
    public class PluginConfigurationManager : IPluginConfigurationManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _filePath;

        public PluginConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
            var pluginDirectory = _configuration.GetValue<string>("PluginsDirectory");

            if (string.IsNullOrEmpty(pluginDirectory))
            {
                throw new ArgumentException("Plugin directory configuration is missing or empty");
            }

            _filePath = Path.Combine(Directory.GetCurrentDirectory(), pluginDirectory);
        }

        public async Task<RepositoryResponse<List<PluginConfig>>> AddPluginAsync(PluginConfig pluginConfig)
        {
            var response = new RepositoryResponse<List<PluginConfig>>();

            try
            {
                var plugins = await GetPluginsConfigsAsync();
                plugins.Data.Add(pluginConfig);
                response = await SavePluginAsync(plugins.Data);

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<RepositoryResponse<List<PluginConfig>>> DeletePluginAsync(string name)
        {
            var response = new RepositoryResponse<List<PluginConfig>>
            {
                Data = new List<PluginConfig>()
            };

            try
            {
                var plugins = await GetPluginsConfigsAsync();

                foreach (var plugin in plugins.Data)
                {
                    if (plugin.Name == name)
                    {
                        response.Data.Add(plugin);
                    }
                }

                plugins.Data.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                var saveResponse = await SavePluginAsync(plugins.Data);

                if (response.IsSuccess)
                {
                    response.Message.Add(Messages.SuccessfullyDeleteMessage);
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<RepositoryResponse<List<PluginConfig>>> GetPluginsConfigsAsync()
        {
            var response = new RepositoryResponse<List<PluginConfig>>();

            try
            {
                if (!File.Exists(_filePath))
                {
                    response.Data = new List<PluginConfig>();
                    response.Message.Add("Plugin directory does not exist");

                    return response;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                response.Data = JsonConvert.DeserializeObject<List<PluginConfig>>(json);

                if (response.Data == null)
                {
                    response.Data = new List<PluginConfig>();
                }

                response.Message.Add(Messages.SuccessfullyGetAllMessage);

                return response;
            }
            catch (IOException ioEx)
            {
                response.IsSuccess = false;
                response.Message.Add(ioEx.Message);
            }
            catch (JsonException jsonEx)
            {
                response.IsSuccess = false;
                response.Message.Add(jsonEx.Message);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<RepositoryResponse<List<PluginConfig>>> SavePluginAsync(List<PluginConfig> pluginConfig)
        {
            var response = new RepositoryResponse<List<PluginConfig>>();

            try
            {
                var json = JsonConvert.SerializeObject(pluginConfig, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, json);

                response.Data = pluginConfig;
                response.Message.Add(Messages.SuccessfullySaveMessage);
            }
            catch (IOException ioEx)
            {
                response.IsSuccess = false;
                response.Message.Add(ioEx.Message);
            }
            catch (JsonException jsonEx)
            {
                response.IsSuccess = false;
                response.Message.Add(jsonEx.Message);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message.Add(ex.Message);
            }

            return response;
        }
    }
}
