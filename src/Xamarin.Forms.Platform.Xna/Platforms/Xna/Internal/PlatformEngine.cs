namespace Xamarin.Forms.Platforms.Xna
{
    using Xamarin.Forms.Platforms.Xna.Renderers;

    public class PlatformEngine
    {
        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderer = VisualElementRenderer.GetRenderer(view);
            return renderer.Measure(new Size(widthConstraint, heightConstraint));
        }
    }
}
