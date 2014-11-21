namespace Xamarin.Forms.Platforms.Xna.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms.Platforms.Xna.Renderers;

    public static class RoutedEventManager
    {
        public static bool HandleRaise(this VisualElementRenderer renderer, Func<VisualElementRenderer, bool> previewHandler, Func<VisualElementRenderer, bool> eventHandler)
        {
            return (previewHandler != null && renderer.RouteToRoot().Reverse().Any(previewHandler)) ||
                   (eventHandler != null && renderer.RouteToRoot().Any(eventHandler));
        }

        static IEnumerable<VisualElementRenderer> RouteToRoot(this VisualElementRenderer renderer)
        {
            VisualElementRenderer current = renderer;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}
