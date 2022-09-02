using System.Security.Claims;

namespace GameStore.Web.Infrastructure.Authorization;

public static class Claims
{
    public const string Id = ClaimTypes.Name;
    public const string Role = ClaimsIdentity.DefaultRoleClaimType;
    public const string UserName = "UserName";
    public const string PublisherName = "PublisherName";
}