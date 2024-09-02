
namespace Messenger.Infrastructure.KeyBuilder;

public class CacheKeyBuilderFactory : ICacheKeyBuilderFactory
{
    public ICacheKeyBuilder Create(Type itemType)
    {
        return new CacheKeyBuilder(itemType);
    }
}
