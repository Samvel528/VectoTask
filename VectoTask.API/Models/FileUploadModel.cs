namespace VectoTask.API.Models
{
    public class FileUploadModel
    {
        public string Name { get; set; }

        public IFormFile File { get; set; }
    }
}
