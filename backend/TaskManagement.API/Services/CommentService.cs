using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Comments;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentResponseDto>> GetCommentsAsync(int taskItemId)
        {
            return await _context.Comments
                .Where(c => c.TaskItemId == taskItemId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.FullName,
                    TaskItemId = c.TaskItemId
                })
                .ToListAsync();
        }

        public async Task<CommentResponseDto> AddCommentAsync(int taskItemId, CreateCommentDto dto, string userId)
        {
            var task = await _context.TaskItems.FindAsync(taskItemId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var comment = new Comment
            {
                Content = dto.Content,
                TaskItemId = taskItemId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentResponseDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                UserName = user.FullName,
                TaskItemId = comment.TaskItemId
            };
        }
    }
}
