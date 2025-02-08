using Amazon.CognitoIdentityProvider.Model;
using System.Security.Claims;

namespace TalkToMe.Helpers
{
    public static class UserHelper
    {
        public static string GetUserId(ClaimsPrincipal user) 
        {
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId is null)
                throw new NotAuthorizedException("User is not authorized");

            return userId;
        }
    }
}
