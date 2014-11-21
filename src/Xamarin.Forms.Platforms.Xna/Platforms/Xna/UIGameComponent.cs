using System.Linq;
using Xamarin.Forms.Platforms.Xna.Input;

namespace Xamarin.Forms.Platforms.Xna
{
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.Platforms.Xna.Renderers;
    using GameTime = Microsoft.Xna.Framework.GameTime;
    using MouseState = Microsoft.Xna.Framework.Input.MouseState;

    public class UIGameComponent : Microsoft.Xna.Framework.DrawableGameComponent, IPlatform
    {
        Page _page;
        IVisualElementRenderer _renderer;
        object _bindingContext;

        public Rectangle? Area;

        public UIGameComponent()
            : base(Forms.Game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if (_renderer != null)
            {
                _page.Layout(Area ?? Forms.Game.GraphicsDevice.Viewport.Bounds.ToXFormsRectangle());
                _renderer.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (_renderer != null)
            {
                _renderer.Update(gameTime);
                Mouse.Update(_renderer);
            }
            base.Update(gameTime);
        }

        public object BindingContext
        {
            get { return _bindingContext; }
            set
            {
                if (_bindingContext == value)
                    return;
                _bindingContext = value;
                if (BindingContextChanged != null)
                    BindingContextChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler BindingContextChanged;

        public IPlatformEngine Engine
        {
            get { return Forms.PlatformEngine; }
        }

        public Page Page { get { return _page; } }

        public void SetPage(Page newRoot)
        {
            _page = newRoot;
            _page.Platform = this;
            _renderer = VisualElementRenderer.Create(newRoot);
        }
    }

    public static class UIGameComponentExtensions
    {
        public static UIGameComponent AsGameComponent(this Page page)
        {
            var component = new UIGameComponent();
            component.SetPage(page);
            return component;
        }

        public static UIGameComponent AsGameComponent(this View view)
        {
            var component = new UIGameComponent();
            component.SetPage(new ContentPage { Content = view });
            return component;
        }
    }
}
