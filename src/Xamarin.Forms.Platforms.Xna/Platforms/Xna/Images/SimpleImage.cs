namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    class SimpleImage : IImage
    {
        Microsoft.Xna.Framework.Graphics.Texture2D _texture;

        public SimpleImage(Texture2D texture)
        {
            _texture = texture;
        }

        public SizeRequest Measure(Size availableSize, SizeRequest contentSize)
        {
            return new SizeRequest(new Size(_texture.Width, _texture.Height), default(Size));
        }

        public void Draw(SpriteBatch spriteBatch, XnaRectangle area, XnaColor color)
        {
            spriteBatch.Draw(_texture, area, color);
        }


        public XnaRectangle GetContentArea(XnaRectangle area)
        {
            return area;
        }

        public void SetState(ISet<State> states)
        {
        }
    }
}
