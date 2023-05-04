using jobboard.Auth;
using jobboard.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace jobboard.Data
{
    public class AuthDbSeeder
    {
        public readonly UserManager<JobBoardUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;


        public AuthDbSeeder(UserManager<JobBoardUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            await AddDefaultRoles();
            await AddAdminUser();
        }
        private async Task AddAdminUser()
        {
            var newAdminUser = new JobBoardUser()
            {
                UserName = "admin",
                Email = "admin@ad.com",
                Name = "admin",
                Surname = "admin",
            };

            var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
            if (existingAdminUser == null)
            {
                var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "!Admin.2023");
                if (createAdminUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newAdminUser, Roles.All);
                }
            }
        }
        private async Task AddDefaultRoles()
        {
            foreach (var role in Roles.All)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }


    }
}
