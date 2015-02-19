namespace Xamarin.Forms.Platforms.Xna.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;

    public class MouseButtonEventArgs : MouseEventArgs
    {
        public readonly Mouse.Button Button;

        public MouseButtonEventArgs(Mouse.Button button, IReadOnlyDictionary<Mouse.Button, ButtonState> buttons, Vector2? position)
            : base(buttons, position)
        {
            Button = button;
        }
    }
}
