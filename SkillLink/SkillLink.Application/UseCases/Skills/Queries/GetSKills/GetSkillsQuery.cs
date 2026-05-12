using MediatR;
using SkillLink.Application.Common;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Skills.Queries.GetSKills
{

    public record GetSkillsQuery(
        int? CategoryId,
        SkillLevel? Level,
        ExchangeMode? Mode,
       
        string? Search,
        int Page = 1,
        int PageSize = 12
    ) : IRequest<PaginatedList<SkillFeedDto>>
    {
       
        public int? CurrentUserId { get; set; }
    }
}
