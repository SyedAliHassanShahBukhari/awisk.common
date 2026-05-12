using awisk.common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace awisk.common.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? Email => User?.FindFirstValue(ClaimTypes.Email)
                             ?? User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        public string? FullName => User?.FindFirstValue(ClaimTypes.Name);
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
        public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];

        public bool IsInRole(string role) =>
            User?.IsInRole(role) ?? false;
    }
}
