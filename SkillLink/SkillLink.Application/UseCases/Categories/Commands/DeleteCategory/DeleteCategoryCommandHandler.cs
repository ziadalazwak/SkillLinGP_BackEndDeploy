using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ISkillLinkDbContext _context;

        public DeleteCategoryCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.SkillCategories
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (category == null)
                throw new Exception("Category not found");

            category.IsDeleted = true;
            category.IsActive  = false;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
