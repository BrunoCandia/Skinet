using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var userName = user.FindFirst(ClaimTypes.Name)?.Value ?? throw new InvalidOperationException("User name claim not found");

            return userName;
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID claim not found");

            return new Guid(userId);
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            var userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? throw new AuthenticationException("User email claim not found");

            return userEmail;
        }

        public static async Task<User> GetUserByEmail(this UserManager<User> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users.FirstOrDefaultAsync(x => x.Email == user.GetUserEmail());

            if (userToReturn is null)
                throw new AuthenticationException("User not found");

            return userToReturn;
        }

        public static async Task<User> GetUserByEmailWithAddress(this UserManager<User> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == user.GetUserEmail());

            if (userToReturn is null)
                throw new AuthenticationException("User not found");

            return userToReturn;
        }
    }
}
