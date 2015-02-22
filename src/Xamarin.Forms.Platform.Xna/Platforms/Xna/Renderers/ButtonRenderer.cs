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
        #region Default Style
        static Color DefaultBackgroundColor = Color.White;
        static Color DefaultTextColor = Color.Black;
        #endregion

        #region Attached Properties
        public static BindableProperty BackgroundImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                r => GetBackgroundImage(r),
                "pack://application/Xamarin.Forms.Platform.WP8;component/Xamarin.Forms.ButtonBackground.xml");

        public static ImageSource GetBackgroundImage(BindableObject obj)
        {
            return (ImageSource)obj.GetValue(BackgroundImageProperty);
        }
        #endregion

        readonly Label Label;
        IControl BackgroundImage;
        Color TextColor;
        Color BackgroundColor;

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
            PropertyTracker.AddHandler(BackgroundImageProperty, Handle_BackgroundImage);

            OnMouseClick += ButtonRenderer_OnMouseClick;
        }

        #region Render

        public override SizeRequest Measure(Size availableSize)
        {
            var lblSize = Label.Measure(VisualState, availableSize, default(SizeRequest));

            if (BackgroundImage != null)
                return BackgroundImage.Measure(VisualState, availableSize, lblSize);

            return lblSize;
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            Rectangle textArea = area;
            if (BackgroundImage != null)
            {
                BackgroundImage.Draw(VisualState, SpriteBatch, area, BackgroundColor);
                textArea = BackgroundImage.GetContentArea(VisualState, area);
            }

            Label.Draw(VisualState, SpriteBatch, textArea, TextColor);
        }

        #endregion

        void ButtonRenderer_OnMouseClick(object sender, MouseEventArgs e)
        {
            var controller = Model as IButtonController;
            if (controller != null)
                controller.SendClicked();
        }

        #region Property Handlers

        void Handle_TextColor(BindableProperty property)
        {
            if (Model.TextColor != default(Xamarin.Forms.Color))
                TextColor = Model.TextColor.ToXnaColor();
            else
                TextColor = DefaultTextColor;
            InvalidateVisual();
        }

        void Handle_Font(BindableProperty property)
        {
            if (Model.FontFamily != null)
                Label.Font = Forms.Game.Content.Load<SpriteFont>(Model.FontFamily);
            else
                Label.Font = LabelRenderer.DefaultFont;

            Label.Scale = (float)Model.FontSize / 18f;
            InvalidateMeasure();
        }

        void Handle_Text(BindableProperty property)
        {
            Label.Text = Model.Text;
            InvalidateMeasure();
        }

        protected override void Handle_BackgroundColor(BindableProperty prop)
        {
            if (Model.BackgroundColor != default(Xamarin.Forms.Color))
                BackgroundColor = Model.BackgroundColor.ToXnaColor();
            else
                BackgroundColor = DefaultBackgroundColor;
            InvalidateVisual();
        }

        async void Handle_BackgroundImage(BindableProperty property)
        {
            BackgroundImage = await GetBackgroundImage(Model).LoadAsync();
            InvalidateMeasure();
        }

        #endregion
    }
}
