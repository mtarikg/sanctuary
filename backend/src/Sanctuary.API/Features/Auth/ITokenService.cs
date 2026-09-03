namespace Sanctuary.API.Features.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}
