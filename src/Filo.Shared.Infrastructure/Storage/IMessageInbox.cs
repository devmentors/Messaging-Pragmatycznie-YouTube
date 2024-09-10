namespace Filo.Shared.Infrastructure.Storage;

public interface IMessageInbox
{
    void Add(object value);
    IEnumerable<object> GetAll();
}