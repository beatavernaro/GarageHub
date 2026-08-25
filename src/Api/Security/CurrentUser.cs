using Application.Interfaces;

namespace GarageHub.Api.Security;

public class CurrentUser(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor;

    public Guid Id
    {
        get
        {
            var userId =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst("sub")?
                    .Value;

            if (string.IsNullOrWhiteSpace(userId) ||
                !Guid.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Usuário não autenticado.");
            }

            return id;
        }
    }
}