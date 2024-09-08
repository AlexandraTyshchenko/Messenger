namespace Messenger.Infrastructure.KeyBuilder;

public class CacheKeyBuilder : ICacheKeyBuilder
{
    private  List<string> _parameters;
    private readonly Type _itemType;

    public CacheKeyBuilder(Type itemType)
    {
        _itemType = itemType;
        _parameters = new List<string>() { itemType.Name };
    }

    public ICacheKeyBuilder AppendParameter(object parameter)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        string? value = parameter.ToString();

        if (value == null)
        {
            throw new ArgumentException($"{nameof(parameter)} cannot be converted to string.");
        }

        _parameters.Add(value);

        return this;
    }

    public string Build()
    {
        string key = string.Join('_', _parameters);
        _parameters = new List<string>() { _itemType.Name };
        return key;
    }
}
