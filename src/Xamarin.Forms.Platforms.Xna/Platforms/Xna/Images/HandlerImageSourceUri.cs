[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.UriImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceUri))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class HandlerImageSourceUri : IImageSourceHandler
    {
        // Baixa a Stream, carrega a imagem de acordo com extensão, via platform services (imageSource.GetStreamAsync())
        // resources do assembly serão representados via pack://
        public Task<IImage> GetImageAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
