namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageSource
    {
        SizeRequest Measure(Size availableSize);
        Texture2D GetImage(Size availableSize);
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<IImageSource> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken);
    }
}
