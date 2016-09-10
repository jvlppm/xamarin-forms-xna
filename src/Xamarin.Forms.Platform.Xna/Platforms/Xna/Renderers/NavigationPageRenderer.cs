[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.NavigationPage),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.NavigationPageRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public class NavigationPageRenderer : VisualElementRenderer<NavigationPage>
    {
        protected override void OnModelLoad(NavigationPage model)
        {
            ((INavigationPageController)model).PopRequested += model_PopRequested;
            ((INavigationPageController)model).PopToRootRequested += model_PopToRootRequested;
            ((INavigationPageController)model).PushRequested += model_PushRequested;
            base.OnModelLoad(model);
        }

        protected override void OnModelUnload(NavigationPage model)
        {
            ((INavigationPageController)model).PopRequested -= model_PopRequested;
            ((INavigationPageController)model).PopToRootRequested -= model_PopToRootRequested;
            ((INavigationPageController)model).PushRequested -= model_PushRequested;
            base.OnModelUnload(model);
        }

        void model_PushRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PushAsync(e.Page);
        }

        void model_PopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PopToRootAsync();
        }

        void model_PopRequested(object sender, NavigationRequestedEventArgs e)
        {
            e.Task = PopAsync();
        }

        Task<bool> PushAsync(Page page)
        {
            var toShow = ((INavigationPageController)Model).StackCopy.First();
            var toHide = ((INavigationPageController)Model).StackCopy.Skip(1).FirstOrDefault();

            return ChangePageAsync(toShow, toHide);
        }

        Task<bool> PopAsync()
        {
            var toHide = ((INavigationPageController)Model).StackCopy.First();
            var toShow = ((INavigationPageController)Model).StackCopy.Skip(1).First();

            return ChangePageAsync(toShow, toHide);
        }

        Task<bool> PopToRootAsync()
        {
            var toHide = ((INavigationPageController)Model).StackCopy.First();
            var toShow = ((INavigationPageController)Model).StackCopy.Last();

            return ChangePageAsync(toShow, toHide);
        }

        async static Task<bool> ChangePageAsync(Page toShow, Page toHide)
        {
            var oldRenderer = GetRenderer(toHide);
            var newRenderer = GetRenderer(toShow);

            newRenderer.IsVisible = true;
            await Task.WhenAll(toShow.FadeTo(1), toHide.FadeTo(0));
            oldRenderer.IsVisible = false;
            return true;
        }
    }
}
