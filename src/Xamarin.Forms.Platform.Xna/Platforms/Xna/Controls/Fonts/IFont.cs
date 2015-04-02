namespace Xamarin.Forms.Platforms.Xna.Controls.Fonts
{
    using Microsoft.Xna.Framework.Graphics;

    public interface IFont : IRegisterable
    {
        SpriteFont GetSpriteFont(float size);
    }
}
