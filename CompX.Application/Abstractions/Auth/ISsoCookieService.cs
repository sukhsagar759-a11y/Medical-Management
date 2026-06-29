using CompX.SharedKernel.Contracts.Auth.Requests;

namespace CompX.Application.Abstractions.Auth;

public interface ISsoCookieService
{
    Task<bool> TryIssueSsoCookieAsync(IssueSsoCookieRequest request, CancellationToken cancellationToken = default);
}
