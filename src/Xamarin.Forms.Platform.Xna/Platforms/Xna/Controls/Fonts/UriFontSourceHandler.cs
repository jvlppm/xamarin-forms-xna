namespace Xamarin.Forms.Platforms.Xna.Controls.Fonts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class UriFontSourceHandler : IFontSourceHandler
    {
        public Task<IFont> GetFontAsync(FontSource source, FontFormat format, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
