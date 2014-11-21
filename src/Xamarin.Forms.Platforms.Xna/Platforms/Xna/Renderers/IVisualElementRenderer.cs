using Xamarin.Forms.Platforms.Xna.Input;
using System.Collections.Immutable;

namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public interface IVisualElementRenderer : IRegisterable
    {
        VisualElement Model { get; set; }

        bool IsVisible { get; set; }

        IVisualElementRenderer Parent { get; set; }
        ImmutableList<IVisualElementRenderer> Children { get; }

        BasicEffect Effect { get; }

        SizeRequest Measure(Size availableSize);
        void Layout(Xamarin.Forms.Rectangle bounds);

        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);

        void Appeared();
        void Disappeared();

        void InvalidateTransformations();
        void InvalidateAlpha();

        void OnMouseEnter();
        void OnMouseLeave();

        bool InterceptMouseDown(Mouse.Button button);
        bool HandleMouseDown(Mouse.Button button);
        bool InterceptMouseUp(Mouse.Button button);
        bool HandleMouseUp(Mouse.Button button);
        bool InterceptClick();
        bool HandleClick();
    }
}
