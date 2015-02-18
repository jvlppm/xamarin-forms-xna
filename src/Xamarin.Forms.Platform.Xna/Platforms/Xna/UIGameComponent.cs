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
        VisualElementRenderer _renderer;
        object _bindingContext;

        public Rectangle? Area;

        public UIGameComponent(Application application)
            : base(Forms.Game)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            Application = application;
            SetPage(Application.MainPage);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_renderer != null)
            {
                Application.MainPage.Layout(Area ?? Forms.Game.GraphicsDevice.Viewport.Bounds.ToXFormsRectangle());
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
