using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.Interfaces
{
    public interface IEmailService
    {

            Task SendOtpEmail(string email,string otp);

    }
}
