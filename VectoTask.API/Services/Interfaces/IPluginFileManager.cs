using VectoTask.API.Models;

namespace VectoTask.API.Services.Interfaces
{
    public interface IPluginFileManager : IPluginManager<RepositoryResponse<FileUploadModel>, FileUploadModel>
    {
    }
}
