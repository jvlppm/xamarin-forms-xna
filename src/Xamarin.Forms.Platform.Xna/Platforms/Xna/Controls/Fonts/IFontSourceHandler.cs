namespace Xamarin.Forms.Platforms.Xna.Controls.Fonts
{
    using System.Threading;
    using System.Threading.Tasks;

    interface IFontSourceHandler : IRegisterable
    {
        Task<IFont> GetFontAsync(FontSource source, FontFormat format, CancellationToken cancellation);
    }
}
