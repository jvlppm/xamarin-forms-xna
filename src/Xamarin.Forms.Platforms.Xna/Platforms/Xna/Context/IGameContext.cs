namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System;

#if !INTERNAL_CONTEXT
    public
#endif
    interface IGameContext
    {
        void Post(Action action);

        ContextOperation Run(IGameOperation operation);

        ContextOperation<T> Run<T>(IGameOperation<T> operation);
    }
}

