using Application.Interfaces;

namespace Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    public Guid Id { get; } =
        Guid.Parse("00000000-0000-0000-0000-000000000001");
}