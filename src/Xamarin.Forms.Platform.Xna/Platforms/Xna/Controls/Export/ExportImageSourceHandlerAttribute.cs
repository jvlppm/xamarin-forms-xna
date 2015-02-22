namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExportImageSourceHandlerAttribute : HandlerAttribute
    {
        public ExportImageSourceHandlerAttribute(Type imageSourceType, Type imageSourceHandlerType)
            : base(imageSourceType, imageSourceHandlerType)
        {
        }
    }
}
