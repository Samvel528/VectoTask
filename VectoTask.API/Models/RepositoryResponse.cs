namespace VectoTask.API.Models
{
    public class RepositoryResponse<T>
    {
        public bool IsSuccess { get; set; } = true;

        public List<string> Message { get; set; } = new List<string>();

        public T Data { get; set; }
    }
}
