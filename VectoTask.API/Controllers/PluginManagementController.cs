using Microsoft.AspNetCore.Mvc;
using VectoTask.API.Constants;
using VectoTask.API.Models;
using VectoTask.API.Services.Interfaces;

namespace VectoTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PluginManagementController : ControllerBase
    {
        private readonly IPluginFileManager _pluginFileManager;
        private readonly IPluginConfigurationManager _pluginConfigurationManager;
        private readonly IConfiguration _configuration;
        private readonly string? _pluginsFilesFolder;

        public PluginManagementController(IPluginFileManager pluginFileManager, IPluginConfigurationManager pluginConfigurationManager, IConfiguration configuration)
        {
            _pluginFileManager = pluginFileManager;
            _pluginConfigurationManager = pluginConfigurationManager;
            _configuration = configuration;
            _pluginsFilesFolder = _configuration.GetValue<string>("PluginsFilesFolder");
        }

        [HttpGet]
        public async Task<IActionResult> GetPlugins()
        {
            var response = await _pluginConfigurationManager.GetPluginsConfigsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("UploadPlugin")]
        public async Task<IActionResult> UploadPlugin([FromForm] FileUploadModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _pluginFileManager.AddPluginAsync(model);

                PluginConfig pluginConfig = new PluginConfig()
                {
                    Name = model.Name,
                    FilePath = $"{_pluginsFilesFolder}/{model.File.FileName}",
                    FileName = model.File.FileName
                };

                await _pluginConfigurationManager.AddPluginAsync(pluginConfig);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
            else
            {
                return BadRequest(Messages.InvalidModelMessage);
            }
        }

        [HttpPost("DeletePlugin")]
        public async Task<IActionResult> DeletePlugin([FromForm] string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var configResponse = await _pluginConfigurationManager.DeletePluginAsync(name);
                var fileResponse = new RepositoryResponse<FileUploadModel>();

                foreach (var plugin in configResponse.Data)
                {
                    fileResponse = await _pluginFileManager.DeletePluginAsync(plugin.FileName);
                }

                if (configResponse.IsSuccess && fileResponse.IsSuccess)
                {
                    return Ok(configResponse);
                }
                else
                {
                    return StatusCode(500, configResponse);
                }
            }
            else
            {
                return BadRequest(Messages.InvalidModelMessage);
            }
        }
    }
}
