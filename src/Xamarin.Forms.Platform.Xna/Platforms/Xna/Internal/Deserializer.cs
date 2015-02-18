[assembly: Xamarin.Forms.Dependency(typeof(Xamarin.Forms.Platforms.Xna.Deserializer))]

namespace Xamarin.Forms.Platforms.Xna
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class Deserializer : IDeserializer
    {
        // TODO: Serialize properties to disk.
        static Task<IDictionary<string, object>> InMemoryProperties;

        static Deserializer()
        {
            InMemoryProperties = Task.FromResult((IDictionary<string, object>)new Dictionary<string, object>());
        }

        public Task<IDictionary<string, object>> DeserializePropertiesAsync()
        {
            return InMemoryProperties;
        }

        public Task SerializePropertiesAsync(IDictionary<string, object> properties)
        {
            InMemoryProperties = Task.FromResult(properties);
            return Task.FromResult(true);
        }
    }
}
