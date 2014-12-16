namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public interface IRenderElement
    {
        SizeRequest Measure(Size availableSize);
        void Draw(SpriteBatch spriteBatch, XnaRectangle area);
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<IRenderElement> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken);
    }
}
