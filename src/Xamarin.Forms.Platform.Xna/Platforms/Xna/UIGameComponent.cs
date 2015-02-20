using Xamarin.Forms.Platforms.Xna.Input;

namespace Xamarin.Forms.Platforms.Xna
{
    using System;
    using Xamarin.Forms;
    using Renderers;
    using GameTime = Microsoft.Xna.Framework.GameTime;

    public class UIGameComponent : Microsoft.Xna.Framework.DrawableGameComponent, IPlatform
    {
        public readonly Application Application;
        public Microsoft.Xna.Framework.Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;

                _position = value;
                InvalidateMeasure();
            }
        }

        public Microsoft.Xna.Framework.Vector2? Size
        {
            get { return _size; }
            set
            {
                if (_size == value)
                    return;

                _size = value;
                InvalidateMeasure();
            }
        }

        public Microsoft.Xna.Framework.Rectangle Bounds
        {
            set
            {
                _position = new Microsoft.Xna.Framework.Vector2(value.X, value.Y);
                _size = new Microsoft.Xna.Framework.Vector2(value.Width, value.Height);
                InvalidateMeasure();
            }
        }

        VisualElementRenderer _renderer;
        object _bindingContext;
        Microsoft.Xna.Framework.Vector2 _position;
        Microsoft.Xna.Framework.Vector2? _size;
        Microsoft.Xna.Framework.Rectangle? _viewportBounds;

        public UIGameComponent(Application application)
            : base(Forms.Game)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            Application = application;
            SetPage(Application.MainPage);
        }

        public void InvalidateMeasure()
        {
            var finalSize = _size;
            if (finalSize == null)
            {
                var size = Application.MainPage.GetSizeRequest(double.PositiveInfinity, double.PositiveInfinity).Request;
                finalSize = new Microsoft.Xna.Framework.Vector2((float)size.Width, (float)size.Height);
            }
            var finalArea = new Rectangle(_position.X, _position.Y, finalSize.Value.X, finalSize.Value.Y);
            if (finalArea != Application.MainPage.Bounds)
                Application.MainPage.Layout(finalArea);

            if (_viewportBounds != GraphicsDevice.Viewport.Bounds)
            {
                _renderer.InvalidateTransformations();
                _viewportBounds = GraphicsDevice.Viewport.Bounds;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (_renderer != null)
            {
                if (_size == null)
                    InvalidateMeasure();
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

        public Page Page { get { return Application.MainPage; } }

        public void SetPage(Page newRoot)
        {
            if (newRoot == Application.MainPage && (_renderer != null || newRoot == null))
                return;

            if (_renderer != null)
                _renderer.Dispose();

            if (newRoot == null)
            {
                Application.MainPage = null;
                _renderer = null;
                return;
            }

            Application.MainPage = newRoot;
            Application.MainPage.Platform = this;
            _renderer = VisualElementRenderer.Create(Application.MainPage);
        }

        public SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint)
        {
            return Application.MainPage.GetSizeRequest(widthConstraint, heightConstraint);
        }
    }

    public static class UIGameComponentExtensions
    {
        static NavigationPage Navigatable(this Page page)
        {
            if (page == null)
                return null;

            return page as NavigationPage ?? new NavigationPage(page);
        }

        public static UIGameComponent AsGameComponent(this Application application)
        {
            return new UIGameComponent(application);
        }

        public static UIGameComponent AsGameComponent(this Page page)
        {
            return new UIGameComponent(new GameApplication
            {
                MainPage = page.Navigatable()
            });
        }

        public static UIGameComponent AsGameComponent(this View view)
        {
            return new UIGameComponent(new GameApplication
            {
                MainPage = new ContentPage { Content = view }.Navigatable()
            });
        }
    }
}
