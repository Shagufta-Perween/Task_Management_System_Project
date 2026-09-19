using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Models;
using TaskManagement.API.Services;
using Xunit;

namespace TaskManagement.Tests
{
    public class TaskServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateTaskAsync_ValidData_CreatesTaskAndSendsNotification()
        {
            // Arrange
            var context = GetDbContext();
            var mockNotify = new Mock<INotificationService>();

            // Seed user and team
            var user = new AppUser { Id = "user-1", FullName = "Test User", Email = "test@example.com" };
            var team = new Team { Id = 1, Name = "Dev Team", CreatedById = "user-1" };
            context.Users.Add(user);
            context.Teams.Add(team);
            await context.SaveChangesAsync();

            var service = new TaskService(context, mockNotify.Object);

            var dto = new CreateTaskDto
            {
                Title = "Test Task Title",
                Description = "Task Description",
                Priority = "High",
                TeamId = 1,
                AssignedToId = "user-1"
            };

            // Act
            var result = await service.CreateTaskAsync(dto, "user-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Task Title", result.Title);
            Assert.Equal("High", result.Priority);
            Assert.Equal("ToDo", result.Status);

            mockNotify.Verify(n => n.CreateNotificationAsync(
                "user-1",
                It.Is<string>(s => s.Contains("assigned a new task")),
                NotificationType.TaskAssigned,
                It.IsAny<int>()
            ), Times.Once);
        }
    }
}
