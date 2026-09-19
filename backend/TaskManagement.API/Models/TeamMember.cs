namespace TaskManagement.API.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
    }
}
