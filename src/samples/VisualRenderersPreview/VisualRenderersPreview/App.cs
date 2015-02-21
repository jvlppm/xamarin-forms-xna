using Xamarin.Forms;

namespace VisualRenderersPreview
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(10, 10),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = {
                        new Button {
                        },
                        new Slider {
                            Minimum = -20,
                            Maximum = 20,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                        }
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
