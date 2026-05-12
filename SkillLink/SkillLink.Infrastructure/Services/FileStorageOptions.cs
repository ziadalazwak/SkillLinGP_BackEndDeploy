using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Infrastructure.Services
{
  
        public class FileStorageOptions
        {
            public string RootPath { get; set; } = string.Empty;   // wwwroot path injected from API
            public string BaseUrl { get; set; } = string.Empty;    // e.g. https://myapp.com
        }
    }

