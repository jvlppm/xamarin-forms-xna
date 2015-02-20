namespace Xamarin.Forms.Platforms.Xna.Input
{
    using Microsoft.Xna.Framework;
    using System;

    public class MouseEventArgs : EventArgs
    {
        public readonly Vector2? Position;

        public MouseEventArgs(Vector2? position)
        {
            Position = position;
        }
    }
}
