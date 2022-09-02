using System;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces;

public interface IOrderMergingService
{
    public Task MergeOrdersAsync(Guid sourceUserId, Guid targetUserId);
}