namespace VectoTask.API.Services.Interfaces
{
    public interface IPluginManager<TR, T>
    {
        Task<TR> AddPluginAsync(T model);

        Task<TR> DeletePluginAsync(string name);
    }
}
