namespace GameStore.Web.Helpers;

public class ViewKeys
{
    public class Games
    {
        public static readonly string Genres = "Genres";
        public static readonly string Platforms = "Platforms";
        public static readonly string Publishers = "Publishers";
        public static readonly string CustomerHasActiveOrder = "CustomerHasActiveOrder";
        public static readonly string PublishedAt = "PublishedAt";
        public static readonly string OrderBy = "OrderBy";
    }

    public class Genres
    {
        public static readonly string ParentGenres = "ParentGenres";
    }

    public class Orders
    {
        public static readonly string HasActiveOrder = "HasActiveOrder";
        public static readonly string Shippers = "Shippers";
    }

    public class Users
    {
        public static readonly string Publishers = "Publishers";
        public static readonly string IsCanDelete = "IsCanDelete";
    }
}