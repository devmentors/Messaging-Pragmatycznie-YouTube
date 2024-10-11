namespace Filo.Shared.Infrastructure.Http;

public interface IMulticastHttpClient
{
    Task PostInSequenceAsync<TPayload>(Uri[] addresses, TPayload payload, CancellationToken cancellationToken);
    Task PostInParallelAsync<TPayload>(Uri[] addresses, TPayload payload, CancellationToken cancellationToken);
}