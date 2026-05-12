using MediatR;
using SkillLink.Application.UseCases.Categories.Queries.GetCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Commands.UpdateCategory
{
   

    public record UpdateCategoryCommand(
        int Id,
        string? Name,
        string? Description,
        bool? IsActive
    ) : IRequest<CategoryDto>;
}
