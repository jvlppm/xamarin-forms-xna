namespace Xamarin.Forms.Platforms.Xna
{
    using Xamarin.Forms.Platforms.Xna.Renderers;

    class PlatformEngine : IPlatformEngine
    {
        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderer = VisualElementRenderer.GetRenderer(view);
            return renderer.Measure(new Size(widthConstraint, heightConstraint));
        }

        public bool Supports3D
        {
            // TODO: Convert mouse clicks to 3D space and back
            get { return false; }
        }
    }
}
