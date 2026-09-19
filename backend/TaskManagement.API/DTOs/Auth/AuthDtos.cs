using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Auth
{
    /// <summary>
    /// User registration request parameters
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Full name of the user (e.g. John Doe)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Valid email address used for login and notifications
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Account password (minimum 6 characters)
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// User login request parameters
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Registered user email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User account password
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Authentication response payload returned upon successful login or registration
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Bearer JWT token to be included in Authorization headers
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Unique user identifier (GUID format)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// User's registered email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's full display name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Assigned system role (Admin, Manager, or User)
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration UTC timestamp
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
