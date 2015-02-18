namespace Xamarin.Forms.Platforms.Xna.Resources
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    class EmbeddedContent : ContentManager
    {
        Assembly _assembly;

        public EmbeddedContent(Assembly assembly, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _assembly = assembly;
        }

        protected override System.IO.Stream OpenStream(string assetName)
        {
            return GetStream(assetName);
        }

        public System.IO.Stream GetStream(string assetName)
        {
            var finalName = "." + assetName;

            foreach (var res in _assembly.GetManifestResourceNames())
            {
                if (Regex.IsMatch(res, Regex.Escape(finalName) + @"(\.[^.]+)?"))
                    return _assembly.GetManifestResourceStream(res);
            }
            return base.OpenStream(assetName);
        }
    }
}
