using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Skills.Queries.GetSKills
{
    public class SkillFeedDto
    {

        public int Id { get; set; }
        /// <summary>The UserSkill (provider-offering) record ID — use this to identify the exact provider offering.</summary>
        public int UserSkillId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProviderName { get; set; } = null!;
        public int providerId { get; set; }
        public SkillLevel Level { get; set; }
        public ExchangeMode ExchangeMode { get; set; }
        public int? CreditCost { get; set; }
        public string ImageUrl { get; set; } = null!;   
        public int? sessionDuration { get; set; } =null;

        public SkillFeedDto(int id, int userSkillId, string title, ExchangeMode exchangeMode, int CreditCost, string ImageUrl, int sessionDuration)
        {
            Id = id;
            UserSkillId = userSkillId;
            Title = title;
          
            ExchangeMode = exchangeMode;
            this.CreditCost = CreditCost;
            this.ImageUrl= ImageUrl;
            this.sessionDuration=sessionDuration;
        }
      


    }
}
