using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }                 
        public DateTime CreatedAt { get; set; }     
        public DateTime? UpdatedAt { get; set; }   
        public bool IsDeleted { get; set; }         

       
        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
