namespace AgroScan.API.Utilities
{
    public interface IFileUtility
    {
        Task<string> ReadFileAsync(string filePath);
        Task<byte[]> ReadFileBytesAsync(string filePath);
        Task WriteFileAsync(string filePath, string content);
        Task WriteFileBytesAsync(string filePath, byte[] content);
        Task<bool> FileExistsAsync(string filePath);
        Task<long> GetFileSizeAsync(string filePath);
        Task<DateTime> GetFileLastModifiedAsync(string filePath);
        Task DeleteFileAsync(string filePath);
        Task<string[]> GetFilesInDirectoryAsync(string directoryPath, string searchPattern = "*");
        Task CreateDirectoryAsync(string directoryPath);
    }
}
