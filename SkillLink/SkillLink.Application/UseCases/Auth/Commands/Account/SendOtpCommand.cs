using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public record SendOtpCommand(string Email) : IRequest<Unit>;
}
