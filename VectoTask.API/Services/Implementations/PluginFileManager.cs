using VectoTask.API.Constants;
using VectoTask.API.Models;
using VectoTask.API.Services.Interfaces;

namespace VectoTask.API.Services.Implementations
{
    public class PluginFileManager : IPluginFileManager
    {
        private readonly IConfiguration _configuration;
        private readonly string? _pluginsFilesFolder;

        public PluginFileManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _pluginsFilesFolder = _configuration.GetValue<string>("PluginsFilesFolder");
        }

        public async Task<RepositoryResponse<FileUploadModel>> AddPluginAsync(FileUploadModel fileUploadModel)
        {
            RepositoryResponse<FileUploadModel> response = new RepositoryResponse<FileUploadModel>
            {
                Data = fileUploadModel
            };

            try
            {
                var filePath = Path.Combine(_pluginsFilesFolder, fileUploadModel.File.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUploadModel.File.CopyToAsync(stream);
                }

                response.Message.Add(Messages.SuccessefullyUploadMessage);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message.Add(ex.Message);
            }

            return response;
        }

        public async Task<RepositoryResponse<FileUploadModel>> DeletePluginAsync(string name)
        {
            RepositoryResponse<FileUploadModel> response = new RepositoryResponse<FileUploadModel>();

            try
            {
                var filePath = Path.Combine(_pluginsFilesFolder, name);

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }

                response.Message.Add(Messages.SuccessefullyUploadMessage);
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
