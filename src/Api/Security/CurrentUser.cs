using Application.Interfaces;

namespace Api.Security;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? Id
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirst("sub")?
                .Value;

            return Guid.TryParse(userId, out var id)
                ? id
                : null;
        }
    }
}