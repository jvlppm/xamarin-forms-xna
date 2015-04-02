namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExportSourceHandlerAttribute : HandlerAttribute
    {
        public ExportSourceHandlerAttribute(Type sourceType, Type sourceHandlerType)
            : base(sourceType, sourceHandlerType)
        {
        }
    }
}
