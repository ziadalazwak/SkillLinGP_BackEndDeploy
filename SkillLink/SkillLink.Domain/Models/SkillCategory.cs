using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public class SkillCategory:BaseEntity
    {


    
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public string ? ImageUrl { get; set; }  

        // Navigation properties
        public ICollection<Skill> Skills { get; set; } = [];
    }
}
