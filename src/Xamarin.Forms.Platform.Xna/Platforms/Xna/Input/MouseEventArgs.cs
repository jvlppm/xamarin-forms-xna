namespace Xamarin.Forms.Platforms.Xna.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;

    public class MouseEventArgs : EventArgs
    {
        public readonly IReadOnlyDictionary<Mouse.Button, ButtonState> ButtonsState;
        public readonly Vector2? Position;

        public MouseEventArgs(IReadOnlyDictionary<Mouse.Button, ButtonState> buttons, Vector2? position)
        {
            ButtonsState = buttons;
            Position = position;
        }
    }
}
