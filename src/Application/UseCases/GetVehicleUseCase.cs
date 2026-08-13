using System;
using System.Threading.Tasks;
using GarageHub.Domain.Entities;
using GarageHub.Domain.Interfaces;

namespace GarageHub.Application.UseCases;

public class GetVehicleUseCase
{
    private readonly IVehicleRepository _repository;

    public GetVehicleUseCase(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public Task<Vehicle?> ExecuteAsync(Guid id) => _repository.GetByIdAsync(id);
}
