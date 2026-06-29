namespace CompX.Application.Abstractions.Auth;

public interface ITokenService
{
    string CreateToken(string subjectId, string email, IReadOnlyCollection<string> roles);
}
