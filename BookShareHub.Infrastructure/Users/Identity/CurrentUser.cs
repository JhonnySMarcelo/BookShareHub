using BookShareHub.Application.Users.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BookShareHub.Infrastructure.Users.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var claim = _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(claim))
                    throw new UnauthorizedAccessException();

                return Guid.Parse(claim);
            }
        }
    }
}
