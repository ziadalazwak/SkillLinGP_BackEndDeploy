using MediatR;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Credits.Queries.GetCreditBalance
{
    public class GetCreditBalanceQueryHandler : IRequestHandler<GetCreditBalanceQuery, int>
    {
        private readonly ISkillLinkDbContext _context;

        public GetCreditBalanceQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetCreditBalanceQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user.CreditBalance;
        }
    }
}
