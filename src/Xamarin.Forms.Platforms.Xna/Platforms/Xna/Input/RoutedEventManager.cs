namespace Xamarin.Forms.Platforms.Xna.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms.Platforms.Xna.Renderers;

    public static class RoutedEventManager
    {
        public static bool HandleRaise(this IVisualElementRenderer renderer, Func<IVisualElementRenderer, bool> previewHandler, Func<IVisualElementRenderer, bool> eventHandler)
        {
            return (previewHandler != null && renderer.RouteToRoot().Reverse().Any(previewHandler)) ||
                   (eventHandler != null && renderer.RouteToRoot().Any(eventHandler));
        }

        static IEnumerable<IVisualElementRenderer> RouteToRoot(this IVisualElementRenderer renderer)
        {
            IVisualElementRenderer current = renderer;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}
