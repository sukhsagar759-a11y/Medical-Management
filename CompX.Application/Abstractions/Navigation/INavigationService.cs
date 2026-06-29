using CompX.SharedKernel.Contracts.Navigation.Responses;

namespace CompX.Application.Abstractions.Navigation;

public interface INavigationService
{
    Task<NavigationMenuResponse> GetMenuAsync(string userId, CancellationToken cancellationToken = default);
}

