namespace Xamarin.Forms.Platforms.Xna.Controls.Images
{
    using Controls;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageSourceHandler : IRegisterable
    {
        Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken);
    }
}
