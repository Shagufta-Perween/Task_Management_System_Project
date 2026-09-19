using TaskManagement.API.DTOs.Comments;

namespace TaskManagement.API.Services
{
    public interface ICommentService
    {
        Task<List<CommentResponseDto>> GetCommentsAsync(int taskItemId);
        Task<CommentResponseDto> AddCommentAsync(int taskItemId, CreateCommentDto dto, string userId);
    }
}
