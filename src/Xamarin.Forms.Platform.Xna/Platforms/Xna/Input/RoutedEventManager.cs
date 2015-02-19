namespace Xamarin.Forms.Platforms.Xna.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Renderers;

    public static class RoutedEventManager
    {
        public static bool HandleRaise(this VisualElementRenderer renderer, Func<VisualElementRenderer, bool> previewHandler, Func<VisualElementRenderer, bool> eventHandler)
        {
            return (previewHandler != null && renderer.RouteToRoot().Reverse().Any(previewHandler)) ||
                   (eventHandler != null && renderer.RouteToRoot().Any(eventHandler));
        }

        public static bool HandleRaise<T>(this VisualElementRenderer renderer, Func<VisualElementRenderer, T> argumentSelector, Func<VisualElementRenderer, T, bool> previewHandler, Func<VisualElementRenderer, T, bool> eventHandler)
        {
            var arguments = new Dictionary<VisualElementRenderer, T>();
            return (previewHandler != null && renderer.RouteToRoot().Reverse().Any(e => previewHandler(e, arguments.GetOrAdd(e, argumentSelector)))) ||
                   (eventHandler != null && renderer.RouteToRoot().Any(e => eventHandler(e, arguments.GetOrAdd(e, argumentSelector))));
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

        static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            var value = createValue(key);
            dictionary.Add(key, value);
            return value;
        }
    }
}
