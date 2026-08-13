using System;
using System.Threading.Tasks;
using GarageHub.Domain.Entities;

namespace GarageHub.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
}
