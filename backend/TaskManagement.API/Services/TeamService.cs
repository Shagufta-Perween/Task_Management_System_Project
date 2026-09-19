using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Teams;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TeamService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<TeamResponseDto>> GetTeamsAsync(string userId, string userRole)
        {
            var query = _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .AsQueryable();

            if (userRole == "User")
            {
                // Users only see teams they belong to
                query = query.Where(t => t.Members.Any(m => m.UserId == userId));
            }
            else if (userRole == "Manager")
            {
                // Managers see teams they created or belong to
                query = query.Where(t =>
                    t.CreatedById == userId ||
                    t.Members.Any(m => m.UserId == userId));
            }
            // Admin sees all teams

            var teams = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            var result = new List<TeamResponseDto>();

            foreach (var team in teams)
            {
                result.Add(await MapToDto(team));
            }

            return result;
        }

        public async Task<TeamResponseDto?> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return null;
            return await MapToDto(team);
        }

        public async Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto dto, string createdById)
        {
            var team = new Team
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            // Add creator as a team member
            var member = new TeamMember
            {
                TeamId = team.Id,
                UserId = createdById,
                JoinedAt = DateTime.UtcNow
            };
            _context.TeamMembers.Add(member);
            await _context.SaveChangesAsync();

            // Reload with includes
            var created = await _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstAsync(t => t.Id == team.Id);

            return await MapToDto(created);
        }

        public async Task<TeamResponseDto> UpdateTeamAsync(int id, UpdateTeamDto dto)
        {
            var team = await _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                throw new KeyNotFoundException("Team not found.");

            if (dto.Name != null) team.Name = dto.Name;
            if (dto.Description != null) team.Description = dto.Description;

            await _context.SaveChangesAsync();
            return await MapToDto(team);
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                throw new KeyNotFoundException("Team not found.");

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task<TeamResponseDto> AddMemberAsync(int teamId, string userId)
        {
            var team = await _context.Teams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException("Team not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            var existingMember = team.Members.FirstOrDefault(m => m.UserId == userId);
            if (existingMember != null)
                throw new InvalidOperationException("User is already a member of this team.");

            var member = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.TeamMembers.Add(member);
            await _context.SaveChangesAsync();

            // Reload
            var updated = await _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstAsync(t => t.Id == teamId);

            return await MapToDto(updated);
        }

        public async Task RemoveMemberAsync(int teamId, string userId)
        {
            var member = await _context.TeamMembers
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);

            if (member == null)
                throw new KeyNotFoundException("Team member not found.");

            _context.TeamMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        private async Task<TeamResponseDto> MapToDto(Team team)
        {
            var memberDtos = new List<TeamMemberDto>();
            foreach (var member in team.Members)
            {
                var roles = await _userManager.GetRolesAsync(member.User);
                memberDtos.Add(new TeamMemberDto
                {
                    UserId = member.UserId,
                    FullName = member.User.FullName,
                    Email = member.User.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "User",
                    JoinedAt = member.JoinedAt
                });
            }

            return new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                CreatedById = team.CreatedById,
                CreatedByName = team.CreatedBy?.FullName ?? "",
                MemberCount = team.Members.Count,
                Members = memberDtos
            };
        }
    }
}
