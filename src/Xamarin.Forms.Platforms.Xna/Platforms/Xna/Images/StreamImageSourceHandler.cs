[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.StreamImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Context;
    using Controls;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class StreamImageSourceHandler : IImageSourceHandler
    {
        public async Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var streamSorce = (StreamImageSource)imageSource;
            var stream = await Forms.UpdateContext.Wait(streamSorce.Stream(cancellationToken));
            return await ImageFactory.CreateFromStream(stream, format, cancellationToken);
        }
    }
}
