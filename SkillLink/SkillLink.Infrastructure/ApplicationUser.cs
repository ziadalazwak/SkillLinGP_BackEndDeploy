using Microsoft.AspNetCore.Identity;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Infrastructure
{
    public class ApplicationUser:IdentityUser
    {
     
        public int? UserId { get; set; } 
       

    }
}
