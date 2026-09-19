namespace TaskManagement.API.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public string CreatedById { get; set; } = string.Empty;
        public AppUser CreatedBy { get; set; } = null!;

        // Navigation properties
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
