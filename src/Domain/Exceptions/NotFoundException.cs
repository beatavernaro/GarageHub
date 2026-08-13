namespace GarageHub.Domain.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
}