namespace Sample.Controls
{
    using System;
    using Xamarin.Forms;

    public enum ImageButtonState
    {
        Normal,
        Over,
        Pressed,
        Pressing
    }

    public class ImageButton : Label
    {
        public static BindableProperty StateProperty = BindableProperty.Create<ImageButton, ImageButtonState>(p => p.State, defaultValue: ImageButtonState.Normal);
        public static BindableProperty ImageProperty = BindableProperty.Create<ImageButton, ImageSource>(p => p.Image, defaultValue: null);
        public static BindableProperty ImageOpacityProperty = BindableProperty.Create<ImageButton, float>(p => p.ImageOpacity, defaultValue: 1);

        public ImageButtonState State
        {
            get { return (ImageButtonState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public float ImageOpacity
        {
            get { return (float)GetValue(ImageOpacityProperty); }
            set { SetValue(ImageOpacityProperty, value); }
        }

        public ImageButton()
        {
            XAlign = TextAlignment.Center;
            YAlign = TextAlignment.Center;
            BindingContext = this;
        }

        public event EventHandler OnClick;

        public void FireClicked()
        {
            if (OnClick != null)
                OnClick(this, EventArgs.Empty);
        }
    }
}

