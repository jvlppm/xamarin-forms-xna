namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    public interface IVisualElementRenderer : IRegisterable
    {
        VisualElement Model { get; set; }

        bool IsVisible { get; set; }

        IVisualElementRenderer Parent { get; set; }
        IEnumerable<IVisualElementRenderer> Children { get; }


        SizeRequest Measure(Size availableSize);
        void Layout(Xamarin.Forms.Rectangle bounds);

        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);

        void Appeared();
        void Disappeared();

        void InvalidateTransformations();
        void InvalidateAlpha();
    }
}
