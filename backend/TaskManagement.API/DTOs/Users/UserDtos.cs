using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Users
{
    /// <summary>
    /// User profile response DTO
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// Unique user ID (GUID format)
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Registered email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// System role assigned to user (Admin, Manager, User)
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Registration timestamp (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Parameters for changing a user's system role
    /// </summary>
    public class UpdateUserRoleDto
    {
        /// <summary>
        /// New role name to assign. Allowed values: Admin, Manager, User.
        /// </summary>
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
