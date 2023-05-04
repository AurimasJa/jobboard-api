using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace jobboard.Auth
{
    public class CompanyOwnerAuthorizationHandler : AuthorizationHandler<CompanyRequirement, ICompanyOwnedResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompanyRequirement requirement, 
            ICompanyOwnedResource resource)
        {
            if (context.User.IsInRole(Roles.Administratorius) ||
                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.CompanyId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public record CompanyRequirement : IAuthorizationRequirement;
}