using Sample.Controls;
using Sample.Converters;
using Xamarin.Forms;

namespace Sample.UI
{
    class MainPage : ContentPage
    {
        public MainPage()
        {
            var logo = new Xamarin.Forms.Image
            {
                HorizontalOptions = Xamarin.Forms.LayoutOptions.Center,
                VerticalOptions = Xamarin.Forms.LayoutOptions.CenterAndExpand,
                Source = "no-idea",
                HeightRequest = 350
            };

            var startButton = new ImageButton
            {
                HorizontalOptions = Xamarin.Forms.LayoutOptions.Center,
                VerticalOptions = Xamarin.Forms.LayoutOptions.CenterAndExpand,
                Text = "Grid Menu",
                WidthRequest = 150,
                ContinuousClick = true
            };
            startButton.SetBinding(ImageButton.ImageProperty, "State", converter: new ButtonImageConverter());

            startButton.OnClick += startButton_OnClick;

            Content = new Xamarin.Forms.StackLayout
            {
                Orientation = Xamarin.Forms.StackOrientation.Vertical,
                VerticalOptions = Xamarin.Forms.LayoutOptions.FillAndExpand,
                HorizontalOptions = Xamarin.Forms.LayoutOptions.FillAndExpand,

                Children =
                {
                    logo,
                    startButton,
                }
            };
        }

        async void startButton_OnClick(object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            button.IsEnabled = false;
            await Navigation.PushAsync(new GridMenu());
            button.IsEnabled = true;
        }
    }
}
