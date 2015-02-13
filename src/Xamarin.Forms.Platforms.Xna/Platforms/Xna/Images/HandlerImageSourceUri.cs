[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.UriImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceUri))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class HandlerImageSourceUri : IImageSourceHandler
    {
        // Baixa a Stream, carrega a imagem de acordo com extensão, via platform services (imageSource.GetStreamAsync())
        // resources do assembly serão representados via pack://
        public async Task<IImage> GetImageAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            var uriSource = (UriImageSource)imageSource;
            var stream = await uriSource.GetStreamAsync();
            if (stream == null)
                throw new ArgumentException("Resource not found");
            var format = ImageFactory.DetectFormat(uriSource.Uri.ToString());
            return await ImageFactory.CreateFromStream(stream, format);
        }
    }
}
