using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AlphaIDEntityFramework.EntityFramework;

internal static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        ValueConverter<T, string> converter = new(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<T>(v)!);

        ValueComparer<T> comparer = new(
            (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
            v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
            v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v))!);

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}
