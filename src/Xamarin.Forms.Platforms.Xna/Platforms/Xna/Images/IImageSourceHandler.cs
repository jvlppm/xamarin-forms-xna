namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageSourceHandler : IRegisterable
    {
        Task<IImage> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken);
    }
}
