namespace awisk.common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName, string? folder = null, CancellationToken ct = default);
        Task<Stream?> GetAsync(string filePath, CancellationToken ct = default);
        Task<bool> DeleteAsync(string filePath, CancellationToken ct = default);
        Task<bool> ExistsAsync(string filePath, CancellationToken ct = default);
        string GetPublicUrl(string filePath);
    }
}
