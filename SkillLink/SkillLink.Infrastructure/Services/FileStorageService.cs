using Microsoft.Extensions.Options;
using SkillLink.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Infrastructure.Services
{
    // Infrastructure/Services/FileStorageService.cs
    public class FileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;

        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public  async Task<string> SaveAsync(
            Stream fileStream,
            string fileName,
            string folder,
            CancellationToken ct = default)
        {
            var folderPath = Path.Combine(_options.RootPath, "uploads", folder);
            Directory.CreateDirectory(folderPath);

            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var fullPath = Path.Combine(folderPath, uniqueName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(stream, ct);

            return $"/uploads/{folder}/{uniqueName}";   // relative URL back to caller
        }

        public async Task DeleteAsync(string relativePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_options.RootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            await Task.CompletedTask;
        }
    }

}