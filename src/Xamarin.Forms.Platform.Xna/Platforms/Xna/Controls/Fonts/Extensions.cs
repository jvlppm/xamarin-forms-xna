namespace Xamarin.Forms.Platforms.Xna.Controls.Fonts
{
    using Context;
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    static class Extensions
    {
        public static async Task<IFont> LoadAsync(this FontSource source, FontFormat format, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null)
                return null;

            var handler = Registrar.Registered.GetHandler<IFontSourceHandler>(source.GetType());
            return await handler.GetFontAsync(source, format, cancellationToken);
        }
    }
}
