using System.ComponentModel;
using System.Globalization;

namespace NotionExporter.Options;

public enum Operation
{
    Export,
}

public class OperationConverter : TypeConverter
{
    private readonly Dictionary<string, Operation> _lookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "e", Operation.Export },
        { "export", Operation.Export },
    };

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            var result = _lookup.TryGetValue(stringValue, out var operation);
            if (!result)
            {
                const string format = "The value '{0}' is not a valid operation.";
                var message = string.Format(CultureInfo.InvariantCulture, format, value);
                throw new InvalidOperationException(message);
            }

            return operation;
        }

        throw new NotSupportedException("Can't convert value to operation.");
    }
}