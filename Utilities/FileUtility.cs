using System.IO;

namespace AgroScan.API.Utilities
{
    public class FileUtility : IFileUtility
    {
        private readonly ILogger<FileUtility> _logger;

        public FileUtility(ILogger<FileUtility> logger)
        {
            _logger = logger;
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                if (!await FileExistsAsync(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<byte[]> ReadFileBytesAsync(string filePath)
        {
            try
            {
                if (!await FileExistsAsync(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file bytes: {FilePath}", filePath);
                throw;
            }
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                await File.WriteAllTextAsync(filePath, content);
                _logger.LogInformation("Successfully wrote file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task WriteFileBytesAsync(string filePath, byte[] content)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                await File.WriteAllBytesAsync(filePath, content);
                _logger.LogInformation("Successfully wrote file bytes: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing file bytes: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            return await Task.FromResult(File.Exists(filePath));
        }

        public async Task<long> GetFileSizeAsync(string filePath)
        {
            try
            {
                if (!await FileExistsAsync(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var fileInfo = new FileInfo(filePath);
                return await Task.FromResult(fileInfo.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<DateTime> GetFileLastModifiedAsync(string filePath)
        {
            try
            {
                if (!await FileExistsAsync(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var fileInfo = new FileInfo(filePath);
                return await Task.FromResult(fileInfo.LastWriteTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file last modified: {FilePath}", filePath);
                throw;
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                if (await FileExistsAsync(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Successfully deleted file: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<string[]> GetFilesInDirectoryAsync(string directoryPath, string searchPattern = "*")
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
                }

                var files = Directory.GetFiles(directoryPath, searchPattern);
                return await Task.FromResult(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting files in directory: {DirectoryPath}", directoryPath);
                throw;
            }
        }

        public async Task CreateDirectoryAsync(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    _logger.LogInformation("Successfully created directory: {DirectoryPath}", directoryPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating directory: {DirectoryPath}", directoryPath);
                throw;
            }
        }
    }
}
