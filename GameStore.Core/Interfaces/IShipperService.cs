using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Mongo.Shippers;

namespace GameStore.Core.Interfaces;

public interface IShipperService
{
    public Task<List<Shipper>> GetAll();
}