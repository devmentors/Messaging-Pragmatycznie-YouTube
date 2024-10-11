using System.Net.Http.Json;

namespace Filo.Shared.Infrastructure.Http;

internal class MulticastHttpClient(IHttpClientFactory clientFactory) : IMulticastHttpClient
{
    public async Task PostInSequenceAsync<TPayload>(Uri[] addresses, TPayload payload, CancellationToken cancellationToken)
    {
        foreach (var address in addresses)
        {
            await SendAsync(payload, cancellationToken, address);
        }
    }

    public async Task PostInParallelAsync<TPayload>(Uri[] addresses, TPayload payload, CancellationToken cancellationToken)
    {
        var requestsInFlight = addresses.Select(address => SendAsync(payload, cancellationToken, address));
        await Task.WhenAll(requestsInFlight);
    }
    
    private async Task SendAsync<TPayload>(TPayload payload, CancellationToken cancellationToken, Uri address)
    {
        var response = await clientFactory.CreateClient(address.Host).PostAsJsonAsync(address, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}