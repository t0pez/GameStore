using GameStore.Core.Models.Constants;

namespace GameStore.Web.Infrastructure.Authorization;

public static class ApiRoles
{
    public const string Administrator = Roles.Administrator;
    public const string Manager = $"{Administrator}, {Roles.Manager}";
    public const string Publisher = $"{Manager}, {Roles.Publisher}";
    public const string Moderator = $"{Publisher}, {Roles.Moderator}";
    public const string User = $"{Moderator}, {Roles.User}";
}