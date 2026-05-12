using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.Interfaces
{
   
        public interface IFileStorageService
        {
           public Task<string> SaveAsync(
                Stream fileStream,
                string fileName,
                string folder,
                CancellationToken ct = default);

            Task DeleteAsync(string relativePath, CancellationToken ct = default);
        }
    }

