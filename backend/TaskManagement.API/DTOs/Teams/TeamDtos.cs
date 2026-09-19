using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Teams
{
    /// <summary>
    /// Parameters required to create a new team
    /// </summary>
    public class CreateTeamDto
    {
        /// <summary>
        /// Name of the team (must be unique)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of team's role or department
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Parameters for updating team details
    /// </summary>
    public class UpdateTeamDto
    {
        /// <summary>
        /// Updated team name
        /// </summary>
        [StringLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// Updated description
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Parameters for adding a member to a team
    /// </summary>
    public class AddTeamMemberDto
    {
        /// <summary>
        /// Unique ID of the user to be added
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detailed team response DTO including member list
    /// </summary>
    public class TeamResponseDto
    {
        /// <summary>
        /// Unique team identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Team name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Team description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Creation timestamp (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of user who created the team
        /// </summary>
        public string CreatedById { get; set; } = string.Empty;

        /// <summary>
        /// Full name of team creator
        /// </summary>
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// Total count of active team members
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// List of current team members
        /// </summary>
        public List<TeamMemberDto> Members { get; set; } = new();
    }

    /// <summary>
    /// Representation of a member within a team
    /// </summary>
    public class TeamMemberDto
    {
        /// <summary>
        /// User unique ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's system role (Admin, Manager, User)
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the user joined the team (UTC)
        /// </summary>
        public DateTime JoinedAt { get; set; }
    }
}
