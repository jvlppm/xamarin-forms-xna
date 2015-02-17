using Sample.Controls;
using Sample.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample.UI
{
    class GridMenu : ContentPage
    {
        public GridMenu()
        {
            var title = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                XAlign = TextAlignment.Center,
                Text = "Grid Menu"
            };
            Grid.SetColumnSpan(title, 2);

            var scaleButton = CreateButton("Scale", 1, 0);
            scaleButton.OnClick += scaleButton_OnClick;

            var rotateButton = CreateButton("Rotate", 1, 1);
            rotateButton.OnClick += rotateButton_OnClick;

            var backButton = CreateButton("Back", 2, 0);
            backButton.OnClick += startButton_OnClick;

            Content = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                Children =
                {
                    title,
                    scaleButton,
                    rotateButton,
                    backButton,
                }
            };
        }

        async void scaleButton_OnClick(object sender, EventArgs e)
        {
            var btn = (ImageButton)sender;
            btn.IsEnabled = false;
            await btn.ScaleTo(1.5, easing: Easing.CubicIn);
            await btn.ScaleTo(1, easing: Easing.CubicOut);
            btn.IsEnabled = true;
        }

        async void rotateButton_OnClick(object sender, EventArgs e)
        {
            var btn = (ImageButton)sender;
            int rotation = btn.RotationY > 0 ? 0 : 360;
            await btn.RotateYTo(rotation, easing: Easing.CubicInOut, length: 800);
        }

        async void startButton_OnClick(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            button.IsEnabled = false;
            await Navigation.PopAsync();
            button.IsEnabled = true;
        }

        private static ImageButton CreateButton(string text, int row, int column)
        {
            var button = new ImageButton
            {
                Image = "pack://application/ButtonBackground.xml",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = text,
                WidthRequest = 150,
            };
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            return button;
        }
    }
}
