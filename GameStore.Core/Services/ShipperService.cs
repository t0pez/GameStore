using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Mongo.Shippers;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class ShipperService : IShipperService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShipperService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Shipper> Repository => _unitOfWork.GetMongoRepository<Shipper>();

    public async Task<List<Shipper>> GetAll()
    {
        var result = await Repository.GetBySpecAsync();

        return result;
    }
}