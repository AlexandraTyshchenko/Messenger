namespace Messenger.Infrastructure.KeyBuilder;

public interface ICacheKeyBuilderFactory
{
    ICacheKeyBuilder Create(Type type);
}
