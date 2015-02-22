[assembly: Xamarin.Forms.Dependency(typeof(Xamarin.Forms.Platforms.Xna.ResourcesProvider))]

namespace Xamarin.Forms.Platforms.Xna
{
    using Xamarin.Forms;

    class ResourcesProvider : ISystemResourcesProvider
    {
        static readonly ResourceDictionary SystemResources;

        static ResourcesProvider()
        {
            SystemResources = new ResourceDictionary();
        }

        public IResourceDictionary GetSystemResources()
        {
            return SystemResources;
        }
    }
}
