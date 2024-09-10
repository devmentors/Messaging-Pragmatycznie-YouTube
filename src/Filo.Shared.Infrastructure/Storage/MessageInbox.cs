namespace Filo.Shared.Infrastructure.Storage;

internal sealed class MessageInbox : IMessageInbox
{
    private readonly List<object> _storage = new();

    public void Add(object value)
        => _storage.Add(value);

    public IEnumerable<object> GetAll()
        => _storage;
}