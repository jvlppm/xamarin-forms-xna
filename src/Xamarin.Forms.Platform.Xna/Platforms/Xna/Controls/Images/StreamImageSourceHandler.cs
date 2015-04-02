[assembly: Xamarin.Forms.Platforms.Xna.Controls.ExportSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Controls.Images.StreamImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls.Images
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
