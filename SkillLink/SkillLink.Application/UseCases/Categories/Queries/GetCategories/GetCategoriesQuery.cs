using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery:IRequest<List<CategoryDto>>
    {

    }
}
