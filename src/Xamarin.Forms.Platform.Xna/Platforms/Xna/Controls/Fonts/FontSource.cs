namespace Xamarin.Forms.Platforms.Xna.Controls.Fonts
{
    using System;

    public abstract class FontSource
    {
        public static implicit operator FontSource(string source)
        {
            Uri uriSource;
            if (Uri.TryCreate(source, UriKind.Absolute, out uriSource))
                return new UriFontSource { Uri = uriSource };
            return new FileFontSource { File = source };
        }
    }
}
