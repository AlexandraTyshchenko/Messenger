namespace Messenger.Infrastructure.KeyBuilder;

public class CacheKeyBuilder : ICacheKeyBuilder
{
    private readonly List<string> _parameters;

    public CacheKeyBuilder(Type itemType)
    {
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
        return string.Join('_', _parameters);
    }
}
