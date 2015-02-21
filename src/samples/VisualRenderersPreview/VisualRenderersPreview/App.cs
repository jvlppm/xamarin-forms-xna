using Xamarin.Forms;

namespace VisualRenderersPreview
{
    public class App : Application
    {
        public App()
        {
            var tstButton = new Button
            {
                BackgroundColor = Color.FromRgb(53, 186, 243)
            };
            var slider = new Slider
            {
                Minimum = 0,
                Maximum = 1,
                Value = 1,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            tstButton.SetBinding(VisualElement.OpacityProperty, new Binding("Value", source: slider));

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(10, 10),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = {
                        tstButton,
                        slider
                    }
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
