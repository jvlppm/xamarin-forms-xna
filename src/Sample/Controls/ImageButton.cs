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
        public static BindableProperty ImageProperty = BindableProperty.Create<ImageButton, string>(p => p.Image, defaultValue: null);
        public static BindableProperty ContinuousClickProperty = BindableProperty.Create<ImageButton, bool>(p => p.ContinuousClick, defaultValue: false);
        public static BindableProperty ImageOpacityProperty = BindableProperty.Create<ImageButton, float>(p => p.ImageOpacity, defaultValue: 1);

        public ImageButtonState State
        {
            get { return (ImageButtonState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public bool ContinuousClick
        {
            get { return (bool)GetValue(ContinuousClickProperty); }
            set { SetValue(ContinuousClickProperty, value); }
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

