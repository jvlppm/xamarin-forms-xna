[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.LabelRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LabelRenderer : VisualElementRenderer<Label>
    {
        #region Static
        static SpriteFont DefaultFont;
        static LabelRenderer()
        {
            DefaultFont = Forms.EmbeddedContent.Load<SpriteFont>("DefaultFont");
        }
        #endregion

        #region Attributes
        readonly Controls.Label Control;
        Color TextColor;
        #endregion

        #region Constructors
        public LabelRenderer()
        {
            Control = new Controls.Label();

            PropertyTracker.AddHandler(Label.TextColorProperty, Handle_TextColor);
            PropertyTracker.AddHandler(Label.FontProperty, Handle_Font);
            PropertyTracker.AddHandler(Label.TextProperty, Handle_Text);
            PropertyTracker.AddHandler(Label.XAlignProperty, Handle_Align);
            PropertyTracker.AddHandler(Label.YAlignProperty, Handle_Align);
        }
        #endregion

        #region VisualElementRenderer
        public override SizeRequest Measure(Size availableSize)
        {
            return Control.Measure(VisualState, availableSize, default(SizeRequest));
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            Control.Draw(VisualState, SpriteBatch, area, TextColor);
        }
        #endregion

        #region Property Handlers

        void Handle_TextColor(BindableProperty property)
        {
            TextColor = Model.TextColor.ToXnaColor();
            InvalidateVisual();
        }

        void Handle_Font(BindableProperty property)
        {
            if (Model.FontFamily != null)
                Control.Font = Forms.Game.Content.Load<SpriteFont>(Model.FontFamily);
            else
                Control.Font = DefaultFont;

            Control.Scale = (float)Model.FontSize / 14f;
            InvalidateMeasure();
        }

        void Handle_Text(BindableProperty property)
        {
            Control.Text = Model.Text;
            InvalidateMeasure();
        }

        void Handle_Align(BindableProperty property)
        {
            Control.XAlign = Model.XAlign;
            Control.YAlign = Model.YAlign;
            InvalidateVisual();
        }

        #endregion
    }
}
