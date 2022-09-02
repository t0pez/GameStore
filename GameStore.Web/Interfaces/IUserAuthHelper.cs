using System;
using System.Threading.Tasks;

namespace GameStore.Web.Interfaces;

public interface IUserAuthHelper
{
    public Task<bool> CanViewAndEditAsync(Guid idOfUserToEdit);
    public Task<bool> CanDeleteAsync(Guid idOfUserToDelete);
    public Task<bool> IsSameUserAsync(Guid userId);
}