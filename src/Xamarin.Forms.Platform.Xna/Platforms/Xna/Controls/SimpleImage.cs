namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    class SimpleImage : IControl
    {
        readonly Texture2D _texture;

        public SimpleImage(Texture2D texture)
        {
            _texture = texture;
        }

        public SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize)
        {
            return new SizeRequest(new Size(_texture.Width, _texture.Height), default(Size));
        }

        public void Draw(ISet<State> states, SpriteBatch spriteBatch, Rectangle area, Color color)
        {
            spriteBatch.Draw(_texture, area, color);
        }

        public Rectangle GetContentArea(ISet<State> states, Rectangle area)
        {
            return area;
        }
    }
}
