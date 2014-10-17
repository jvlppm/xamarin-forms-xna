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

    public class NavigationPageRenderer : VisualElementRenderer<Xamarin.Forms.NavigationPage>
    {
        protected override void OnModelLoad(Xamarin.Forms.NavigationPage model)
        {
            model.PopRequested += model_PopRequested;
            model.PopToRootRequested += model_PopToRootRequested;
            model.PushRequested += model_PushRequested;
            base.OnModelLoad(model);
        }

        protected override void OnModelUnload(Xamarin.Forms.NavigationPage model)
        {
            model.PopRequested -= model_PopRequested;
            model.PopToRootRequested -= model_PopToRootRequested;
            model.PushRequested -= model_PushRequested;
            base.OnModelUnload(model);
        }

        void model_PushRequested(object sender, Xamarin.Forms.NavigationRequestedEventArgs e)
        {
            e.Task = PushAsync(e.Page);
        }

        void model_PopToRootRequested(object sender, Xamarin.Forms.NavigationRequestedEventArgs e)
        {
            e.Task = PopToRootAsync();
        }

        void model_PopRequested(object sender, Xamarin.Forms.NavigationRequestedEventArgs e)
        {
            e.Task = PopAsync();
        }

        Task<bool> PushAsync(Xamarin.Forms.Page page)
        {
            var toShow = Model.StackCopy.First();
            var toHide = Model.StackCopy.Skip(1).FirstOrDefault();

            return ChangePageAsync(toShow, toHide);
        }

        Task<bool> PopAsync()
        {
            var toHide = Model.StackCopy.First();
            var toShow = Model.StackCopy.Skip(1).First();

            return ChangePageAsync(toShow, toHide);
        }

        Task<bool> PopToRootAsync()
        {
            var toHide = Model.StackCopy.First();
            var toShow = Model.StackCopy.Last();

            return ChangePageAsync(toShow, toHide);
        }

        async static Task<bool> ChangePageAsync(Xamarin.Forms.Page toShow, Xamarin.Forms.Page toHide)
        {
            var oldRenderer = VisualElementRenderer.GetRenderer(toHide);
            var newRenderer = VisualElementRenderer.GetRenderer(toShow);

            newRenderer.IsVisible = true;
            await Task.WhenAll(toShow.FadeTo(1), toHide.FadeTo(0));
            oldRenderer.IsVisible = false;
            return true;
        }
    }
}
