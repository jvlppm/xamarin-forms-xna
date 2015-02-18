#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

    public class DeferredOperation : GameOperation
    {
        public override bool Continue(GameTime gameTime)
        {
            return !IsCompleted;
        }

        new public void SetCompleted()
        {
            base.SetCompleted();
        }

        new public void SetError(Exception ex)
        {
            base.SetError(ex);
        }
    }

    public class DeferredOperation<T> : GameOperation<T>
    {
        public override bool Continue(GameTime gameTime)
        {
            return !IsCompleted;
        }

        new public void SetResult(T result)
        {
            base.SetResult(result);
        }

        new public void SetError(Exception ex)
        {
            base.SetError(ex);
        }
    }
}
#endif
