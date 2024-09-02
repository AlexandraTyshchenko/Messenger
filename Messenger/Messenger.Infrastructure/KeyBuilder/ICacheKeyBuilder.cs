namespace Messenger.Infrastructure.KeyBuilder;

public interface ICacheKeyBuilder
{
    ICacheKeyBuilder AppendParameter(object parameter);
    string Build();
}
