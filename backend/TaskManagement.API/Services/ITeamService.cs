using TaskManagement.API.DTOs.Teams;

namespace TaskManagement.API.Services
{
    public interface ITeamService
    {
        Task<List<TeamResponseDto>> GetTeamsAsync(string userId, string userRole);
        Task<TeamResponseDto?> GetTeamByIdAsync(int id);
        Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto dto, string createdById);
        Task<TeamResponseDto> UpdateTeamAsync(int id, UpdateTeamDto dto);
        Task DeleteTeamAsync(int id);
        Task<TeamResponseDto> AddMemberAsync(int teamId, string userId);
        Task RemoveMemberAsync(int teamId, string userId);
    }
}
