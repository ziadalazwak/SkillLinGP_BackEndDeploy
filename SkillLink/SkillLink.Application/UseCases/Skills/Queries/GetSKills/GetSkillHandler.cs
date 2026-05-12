using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Skills.Queries.GetSKills;
using SkillLink.Domain.Models;
using System;


public class GetSkillsHandler : IRequestHandler<GetSkillsQuery, PaginatedList<SkillFeedDto>>
{
    private readonly ISkillLinkDbContext _context;

    public GetSkillsHandler(ISkillLinkDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<SkillFeedDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        // Start with active UserSkills (Offerings) that are not soft-deleted
        var query = _context.UserSkills
            .Include(us => us.Skill)
            .Include(us => us.User)
            .Where(us => us.IsOffering && us.Skill.IsActive && !us.IsDeleted && !us.Skill.IsDeleted && !us.User.IsDeleted)
            .AsQueryable();

        // Filter out the user's own skills
        if (request.CurrentUserId.HasValue)
            query = query.Where(us => us.UserId != request.CurrentUserId.Value);

        // Filter by category
        if (request.CategoryId.HasValue)
            query = query.Where(us => us.Skill.CategoryId == request.CategoryId.Value);

        // Filter by exchange mode
        if (request.Mode.HasValue)
            query = query.Where(us => us.ExchangeMode == request.Mode.Value || us.ExchangeMode == ExchangeMode.Both);

        // Full-text search on title/description
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(us => us.Skill.Title.Contains(request.Search) || (us.Skill.Description != null && us.Skill.Description.Contains(request.Search)));

        var totalCount = await query.CountAsync(cancellationToken);

        var raw = await query
            .OrderByDescending(us => us.Skill.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(us => new
            {
                us.Id,
                us.SkillId,
                us.Skill.Title,
                us.ExchangeMode,
                us.DurationMinutes,
                ImageUrl = us.Skill.ImageUrl ?? "",
                ProviderName = us.User.FullName ?? "Unknown User",
                us.UserId,
                us.Level
            })
            .ToListAsync(cancellationToken);

        var items = raw.Select(us => new SkillFeedDto(
                us.SkillId,
                us.Id,
                us.Title,
                us.ExchangeMode,
                us.DurationMinutes.HasValue
                    ? (int)Math.Round(us.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero)
                    : 0,
                us.ImageUrl,
                us.DurationMinutes ?? 0
            )
            {
                ProviderName = us.ProviderName,
                providerId   = us.UserId,
                Level        = us.Level
            })
            .ToList();

        return new PaginatedList<SkillFeedDto>(items, totalCount, request.Page, request.PageSize);
    }
}
