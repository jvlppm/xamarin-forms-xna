namespace Xamarin.Forms.Platforms.Xna.Input
{
    using Microsoft.Xna.Framework;

    public class MouseButtonEventArgs : MouseEventArgs
    {
        public readonly Mouse.Button Button;

        public MouseButtonEventArgs(Mouse.Button button, Vector2? position)
            : base(position)
        {
            Button = button;
        }
    }
}
