using Microsoft.AspNetCore.Http;
using SkillLink.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillLink.API.ApiDto
{
    public class CreateSkillDto
    {
        public string Title { get; set; } = string.Empty;


        public string? Description { get; set; }

  
        public int CategoryId { get; set; }



        /// <summary>Session duration in minutes</summary>


        /// <summary>In-person, online, or both</summary>
    
    

        public bool IsActive { get; set; } = true;

        public IFormFile? Image { get; set; }
      
      
    }
}
