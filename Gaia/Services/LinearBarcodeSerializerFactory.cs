using System.Collections.Frozen;

namespace Gaia.Services;

public interface ILinearBarcodeSerializerFactory : IFactory<string, ILinearBarcodeSerializer>
{
    ReadOnlyMemory<string> SupportedBarcodes { get; }
}

public sealed class LinearBarcodeSerializerFactory : ILinearBarcodeSerializerFactory
{
    private readonly FrozenDictionary<string, ILinearBarcodeSerializer> _serializers;

    public LinearBarcodeSerializerFactory(IEnumerable<ILinearBarcodeSerializer> serializers)
    {
        _serializers = serializers.ToFrozenDictionary(x => x.Name);
        SupportedBarcodes = _serializers.Keys.OrderBy(x => x).ToArray();
    }

    public ReadOnlyMemory<string> SupportedBarcodes { get; }

    public ILinearBarcodeSerializer Create(string input)
    {
        return _serializers[input];
    }
}
