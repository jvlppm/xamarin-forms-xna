[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceStream))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class HandlerImageSourceStream : IImageSourceHandler
    {
        // Carrega a imagem de tipo Simple
        public Task<IImage> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
