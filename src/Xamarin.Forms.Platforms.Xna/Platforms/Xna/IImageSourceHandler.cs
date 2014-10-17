namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageSourceHandler : IRegisterable
    {
        Task<Texture2D> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken);
    }
}
