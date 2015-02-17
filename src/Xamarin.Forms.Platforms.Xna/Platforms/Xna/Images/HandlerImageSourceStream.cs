[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceStream))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Context;

    public class HandlerImageSourceStream : IImageSourceHandler
    {
        public async Task<IImage> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var streamSorce = (StreamImageSource)imageSource;
            var stream = await Forms.UpdateContext.Wait(streamSorce.Stream(cancellationToken));
            return await ImageFactory.CreateFromStream(stream, format, cancellationToken);
        }
    }
}
