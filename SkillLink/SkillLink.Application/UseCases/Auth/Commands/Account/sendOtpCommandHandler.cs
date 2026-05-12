using MediatR;
using SkillLink.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class sendOtpCommandHandler : IRequestHandler<SendOtpCommand,Unit>
    {
        private readonly IOtpService _otpService;
        public sendOtpCommandHandler(IOtpService otpService)
        {
            _otpService = otpService;
        }
        public async Task<Unit> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            
             await _otpService.SendOtpAsync(request.Email);
            return Unit.Value;


        }
    }
}
