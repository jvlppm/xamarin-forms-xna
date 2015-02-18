namespace Sample.Controls
{
    using System;
    using Xamarin.Forms;

    public class ImageButton : Label
    {
        public static BindableProperty ImageProperty = BindableProperty.Create<ImageButton, ImageSource>(p => p.Image, defaultValue: null);
        public static BindableProperty ImageOpacityProperty = BindableProperty.Create<ImageButton, float>(p => p.ImageOpacity, defaultValue: 1);

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

