[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Button),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.ButtonRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Controls;
    using Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xna;

    public class ButtonRenderer : VisualElementRenderer<Button>
    {
        #region Static
        static SpriteFont DefaultFont;
        static Color DefaultTextColor = Color.Black;
        static IControl Background;
        static ButtonRenderer()
        {
            DefaultFont = Forms.EmbeddedContent.Load<SpriteFont>("DefaultFont");
            var background = new UriImageSource
            {
                Uri = new System.Uri("pack://application/Xamarin.Forms.Platform.WP8;component/ButtonBackground"),
                CachingEnabled = false
            };
            background.LoadAsync(format: ImageFormat.StateList)
                .ContinueWith(t => Background = t.Result);
        }
        #endregion

        readonly Label Label;
        Color TextColor = DefaultTextColor;

        public ButtonRenderer()
        {
            Label = new Label
            {
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Center,
            };

            PropertyTracker.AddHandler(Button.TextColorProperty, Handle_TextColor);
            PropertyTracker.AddHandler(Button.FontProperty, Handle_Font);
            PropertyTracker.AddHandler(Button.TextProperty, Handle_Text);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            var lblSize = Label.Measure(VisualState, availableSize, default(SizeRequest));

            if (Background != null)
                return Background.Measure(VisualState, availableSize, lblSize);

            return lblSize;
        }

        public override bool HandleClick()
        {
            ((IButtonController)Model).SendClicked();
            return true;
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            Rectangle textArea = area;
            if (Background != null)
            {
                Background.Draw(VisualState, SpriteBatch, area, Color.White);
                textArea = Background.GetContentArea(VisualState, area);
            }

            Label.Draw(VisualState, SpriteBatch, textArea, TextColor);
        }

        #region Visual State

        public override void OnMouseLeave()
        {
            RemoveVisualState(Mouse.Pressed);
            base.OnMouseLeave();
        }

        public override bool HandleMouseDown(Mouse.Button button)
        {
            if (button == Mouse.Button.Left)
                AddVisualState(Mouse.Pressed);
            return base.HandleMouseDown(button);
        }

        public override bool HandleMouseUp(Mouse.Button button)
        {
            if (button == Mouse.Button.Left)
                RemoveVisualState(Mouse.Pressed);
            return base.HandleMouseUp(button);
        }

        #endregion

        #region Property Handlers

        void Handle_TextColor(BindableProperty property)
        {
            if (Model.TextColor == default(Xamarin.Forms.Color))
                TextColor = DefaultTextColor;
            else
                TextColor = Model.TextColor.ToXnaColor();
            InvalidateVisual();
        }

        void Handle_Font(BindableProperty property)
        {
            if (Model.FontFamily != null)
                Label.Font = Forms.Game.Content.Load<SpriteFont>(Model.FontFamily);
            else
                Label.Font = DefaultFont;

            Label.Scale = (float)Model.FontSize / 14f;
            InvalidateMeasure();
        }

        void Handle_Text(BindableProperty property)
        {
            Label.Text = Model.Text;
            InvalidateMeasure();
        }

        #endregion
    }
}
