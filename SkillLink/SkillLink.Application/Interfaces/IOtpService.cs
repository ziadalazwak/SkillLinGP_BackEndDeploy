using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.Interfaces
{
    public interface IOtpService
    {
        Task SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string code);
    }
}
