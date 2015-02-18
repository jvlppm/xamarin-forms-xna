namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    public interface IControl
    {
        SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize);
        void Draw(ISet<State> states, SpriteBatch spriteBatch, Rectangle area, Color color);
        Rectangle GetContentArea(ISet<State> states, Rectangle area);
    }
}
