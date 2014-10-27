using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    enum RoutingStrategy
    {
        /// <summary>
        /// The routed event uses a bubbling strategy, where the event instance routes upwards through the tree, from event source to root. 
        /// </summary>
        Bubble,
        /// <summary>
        /// The routed event uses a tunneling strategy, where the event instance routes downwards through the tree, from root to source element. 
        /// </summary>
        Tunnel
    }

    class RoutedEvent
    {
        public RoutingStrategy RoutingStrategy;
    }
}
