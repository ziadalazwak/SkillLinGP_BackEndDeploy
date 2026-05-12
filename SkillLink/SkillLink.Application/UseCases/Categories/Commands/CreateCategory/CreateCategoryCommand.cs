using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand:IRequest<int>
    {
        public string? Name { get; set; } 


        public string? Description { get; set; }

        public string? IconUrl { get; set; }

     
        public string? ImageUrl { get; set; }

    }
}
