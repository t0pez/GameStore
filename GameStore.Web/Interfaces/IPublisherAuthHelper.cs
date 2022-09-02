using System.Threading.Tasks;

namespace GameStore.Web.Interfaces;

public interface IPublisherAuthHelper
{
    public Task<bool> CanEditAsync(string publisherName);
}