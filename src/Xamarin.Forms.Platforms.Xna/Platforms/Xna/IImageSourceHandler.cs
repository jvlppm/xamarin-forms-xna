namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
    using XnaColor = Microsoft.Xna.Framework.Color;

    public interface IRenderElement
    {
        SizeRequest Measure(Size availableSize, SizeRequest contentSize);
        void Draw(SpriteBatch spriteBatch, XnaRectangle area, XnaColor color);
        XnaRectangle GetContentArea(XnaRectangle area);
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<IRenderElement> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken);
    }
}
