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
            SystemResources.Add(GetDefaultLabelStyle());
            SystemResources.Add(GetDefaultButtonStyle());
            SystemResources.Add(GetDefaultPageStyle());
        }

        static Style GetDefaultLabelStyle()
        {
            var style = new Style(typeof(Label));
            style.Setters.Add(Label.TextColorProperty, Color.Black);
            style.Setters.Add(Label.FontSizeProperty, 10);
            return style;
        }

        static Style GetDefaultButtonStyle()
        {
            var style = new Style(typeof(Button));
            style.Setters.Add(Button.TextColorProperty, Color.Black);
            style.Setters.Add(Button.FontSizeProperty, 10);
            return style;
        }

        static Style GetDefaultPageStyle()
        {
            var style = new Style(typeof(Page));
            style.Setters.Add(VisualElement.BackgroundColorProperty, Color.Black);
            return style;
        }

        public IResourceDictionary GetSystemResources()
        {
            return SystemResources;
        }
    }
}
