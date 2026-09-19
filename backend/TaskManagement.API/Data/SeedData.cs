using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // Seed roles
            string[] roles = { "Admin", "Manager", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            var adminEmail = "admin@taskmanager.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed a manager user
            var managerEmail = "manager@taskmanager.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);
            if (managerUser == null)
            {
                managerUser = new AppUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    FullName = "Project Manager",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(managerUser, "Manager@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }

            // Seed a regular user
            var userEmail = "user@taskmanager.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);
            if (regularUser == null)
            {
                regularUser = new AppUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FullName = "John Doe",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(regularUser, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }

            // Seed default teams if no teams exist
            if (!await context.Teams.AnyAsync())
            {
                var admin = await userManager.FindByEmailAsync(adminEmail);
                var manager = await userManager.FindByEmailAsync(managerEmail);
                var user = await userManager.FindByEmailAsync(userEmail);

                if (admin != null && manager != null)
                {
                    var engTeam = new Team
                    {
                        Name = "Engineering Team",
                        Description = "Core software development and technical operations team",
                        CreatedById = admin.Id,
                        CreatedAt = DateTime.UtcNow
                    };

                    var productTeam = new Team
                    {
                        Name = "Product & Design",
                        Description = "Product management, UI/UX design, and research team",
                        CreatedById = manager.Id,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Teams.AddRange(engTeam, productTeam);
                    await context.SaveChangesAsync();

                    // Add members to Engineering Team
                    context.TeamMembers.AddRange(
                        new TeamMember { TeamId = engTeam.Id, UserId = admin.Id },
                        new TeamMember { TeamId = engTeam.Id, UserId = manager.Id },
                        new TeamMember { TeamId = engTeam.Id, UserId = user!.Id }
                    );

                    // Add members to Product Team
                    context.TeamMembers.AddRange(
                        new TeamMember { TeamId = productTeam.Id, UserId = manager.Id },
                        new TeamMember { TeamId = productTeam.Id, UserId = user.Id }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
