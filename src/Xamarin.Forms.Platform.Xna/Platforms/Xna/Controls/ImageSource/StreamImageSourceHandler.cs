[assembly: Xamarin.Forms.Platforms.Xna.Controls.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Controls.StreamImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using Context;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    class StreamImageSourceHandler : IImageSourceHandler
    {
        public async Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var streamSorce = (StreamImageSource)imageSource;
            var stream = await Forms.UpdateContext.Wait(streamSorce.Stream(cancellationToken));
            return await ImageFactory.CreateFromStream(stream, format, cancellationToken);
        }
    }
}
