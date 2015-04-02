namespace Xamarin.Forms.Platforms.Xna.Controls.Images
{
    using Context;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    static class Extensions
    {
        public static async Task<IControl> LoadAsync(this ImageSource source, CancellationToken cancellationToken = default(CancellationToken), ImageFormat format = ImageFormat.Unknown)
        {
            if (source == null)
                return null;

            var handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType());
            return await handler.GetImageAsync(source, format, cancellationToken);
        }
    }
}
