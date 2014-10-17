namespace Sample.Renderers
{
    using Xamarin.Forms;

    public interface IClickableRenderer
    {
        VisualElement Model { get; }
    }

    public static class ClickableExtensions
    {
        public static Rectangle GetArea(this IClickableRenderer clickable)
        {
            var pos = new Point();
            var current = clickable.Model;
            while (current != null)
            {
                pos = new Point(
                    pos.X + current.Bounds.X + current.TranslationX,
                    pos.Y + current.Bounds.Y + current.TranslationY);

                current = current.ParentView;
            }
            return new Rectangle(pos, clickable.Model.Bounds.Size);
        }
    }
}
