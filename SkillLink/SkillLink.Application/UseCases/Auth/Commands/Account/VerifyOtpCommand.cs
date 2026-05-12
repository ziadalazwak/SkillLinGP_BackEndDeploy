using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class VerifyOtpCommand: IRequest<bool>
    {
        public string Email { get; set; }   
        public string Otp { get; set; }
    }
}
