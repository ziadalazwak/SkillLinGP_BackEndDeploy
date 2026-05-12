using SkillLink.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public record UserSkillDto(
        int SkillId,
        string Title,
        SkillLevel SkillLevel,
        bool IsOffering,
        
         
     ExchangeMode ExchangeMode ,
     int ?DurationMinutes ,
     string DeliveryMode,  int ?creditCost
    );
}
