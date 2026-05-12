namespace awisk.common.Classes
{
    public class FileStorageSettings
    {
        public string BasePath { get; set; } = "uploads";
        public string BaseUrl { get; set; } = string.Empty;
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
        public string[] AllowedExtensions { get; set; } = [];
    }
}
