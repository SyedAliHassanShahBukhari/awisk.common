using awisk.common.Classes;
using awisk.common.Interfaces;

namespace awisk.common.Services
{
    public class LocalFileStorageService(FileStorageSettings settings) : IFileStorageService
    {
        public async Task<string> SaveAsync(Stream fileStream, string fileName, string? folder = null, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(fileStream);
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

            var safeFileName = Path.GetFileName(fileName);
            var relativePath = folder is not null
                ? Path.Combine(folder, safeFileName)
                : safeFileName;

            var fullPath = Path.Combine(settings.BasePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await fileStream.CopyToAsync(fs, ct).ConfigureAwait(false);

            return relativePath.Replace('\\', '/');
        }

        public Task<Stream?> GetAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(settings.BasePath, filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult<Stream?>(null);

            return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public Task<bool> DeleteAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(settings.BasePath, filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string filePath, CancellationToken ct = default) =>
            Task.FromResult(File.Exists(Path.Combine(settings.BasePath, filePath)));

        public string GetPublicUrl(string filePath)
        {
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            return $"{baseUrl}/{filePath.TrimStart('/')}";
        }
    }
}
