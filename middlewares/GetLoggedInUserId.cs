using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace foodOrderingApp.middlewares
{
    public static class GetLoggedInUserId
    {
        public static Guid GetUserIdFromClaims(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException( "Login First");
            }
            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            return userId;
        }
    }
}