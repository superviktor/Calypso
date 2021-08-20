using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calypso.TeamsApp123.Common
{
    public static class Mapper
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions();

        static Mapper()
        {
            Options.Converters.Add(new JsonStringEnumConverter());
        }

        public static T Map<T>(this object source) where T : class
        {
            return source == null
                ? null
                : JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source, source.GetType(), Options), Options);
        }

        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> source) where TIn : class where TOut : class
        {
            return source.Map<IEnumerable<TOut>>();
        }
    }
}