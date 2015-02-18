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

        public UIGameComponent()
            : base(Forms.Game)
        {
            Application = new GameApplication();
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
            Application.MainPage = newRoot as NavigationPage ?? new NavigationPage(newRoot);
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
