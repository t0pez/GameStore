using System.Linq;
using System.Security.Claims;
using GameStore.Core.Models.Constants;
using GameStore.Web.Infrastructure.Authorization;

namespace GameStore.Web.Extensions;

public static class PrincipalExtensions
{
    public static bool TryGetRole(this ClaimsPrincipal user, out string role)
    {
        var claim = user.Claims.FirstOrDefault(c => c.Type == Claims.Role);

        role = claim?.Value;

        return claim is not null;
    }

    public static bool TryGetUserName(this ClaimsPrincipal user, out string userName)
    {
        var claim = user.Claims.FirstOrDefault(c => c.Type == Claims.UserName);

        userName = claim?.Value;

        return claim is not null;
    }

    public static bool TryGetPublisherName(this ClaimsPrincipal user, out string publisherName)
    {
        var claim = user.Claims.FirstOrDefault(c => c.Type == Claims.PublisherName);

        publisherName = claim?.Value;

        return claim is not null;
    }

    public static bool HasPermissionOfLevel(this ClaimsPrincipal user, string roles, string publisherName = null)
    {
        var isUserRoleExists = user.TryGetRole(out var userRole);

        if (isUserRoleExists == false)
        {
            return false;
        }

        var rolesAsArray = roles.Split(", ");

        if (rolesAsArray.Contains(Roles.Publisher) && user.IsInRole(Roles.Publisher))
        {
            return IsPublisherNameCorrect(user, publisherName);
        }

        return rolesAsArray.Contains(userRole);
    }

    private static bool IsPublisherNameCorrect(ClaimsPrincipal user, string publisherName)
    {
        var isPublisherNameExists = user.TryGetPublisherName(out var publisherNameFromClaim);

        if (isPublisherNameExists == false)
        {
            return false;
        }

        return publisherNameFromClaim == publisherName;
    }
}