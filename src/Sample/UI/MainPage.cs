﻿using Sample.Controls;
using Xamarin.Forms;

namespace Sample.UI
{
    class MainPage : ContentPage
    {
        public MainPage()
        {
            var logo = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Source = "no-idea",
                HeightRequest = 350
            };

            var startButton = new ImageButton
            {
                Image = "pack://application/ButtonBackground.xml",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Grid Menu",
                WidthRequest = 150,
            };

            startButton.OnClick += startButton_OnClick;

            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,

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
