using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Commands.UpdateCategory
{
    public class CategoryDto
    {
     
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

     public bool IsActive { get; set; }
        public CategoryDto( int id , string name, string? description, bool isActive)
        {
           Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
        }
       
    }
}
