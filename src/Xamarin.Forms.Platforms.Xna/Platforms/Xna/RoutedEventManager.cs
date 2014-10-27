using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platforms.Xna.Renderers;

namespace Xamarin.Forms.Platforms.Xna
{
    class RoutedEventManager
    {
        static void RaiseEvent(IVisualElementRenderer renderer, RoutedEvent ev)
        {
            switch (ev.RoutingStrategy)
            {
                case RoutingStrategy.Bubble:
                    RouteToRoot(renderer).FirstOrDefault(e => e.Handle(ev));
                    break;
                case RoutingStrategy.Tunnel:
                    RouteToRoot(renderer).Reverse().FirstOrDefault(e => e.Handle(ev));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static IEnumerable<IVisualElementRenderer> RouteToRoot(IVisualElementRenderer renderer)
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
