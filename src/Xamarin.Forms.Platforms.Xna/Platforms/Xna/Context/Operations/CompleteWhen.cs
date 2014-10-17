namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

#if !INTERNAL_CONTEXT
    public
#endif
    class CompleteWhenOperation : GameOperation
    {
        readonly Func<GameTime, bool> _checkCompletion;

        public CompleteWhenOperation(Func<GameTime, bool> checkCompletion)
        {
            if (checkCompletion == null)
                throw new ArgumentNullException("checkCompletion");

            _checkCompletion = checkCompletion;
        }

        public override bool Continue(GameTime gameTime)
        {
            if (IsCompleted)
                return false;

            if (_checkCompletion(gameTime))
            {
                SetCompleted();
                return false;
            }

            return true;
        }
    }

#if !INTERNAL_CONTEXT
    public
#endif
    static class CompleteWhenExtensions
    {
        public static ContextOperation CompleteWhen(this IGameContext context, Func<GameTime, bool> checkCompletion)
        {
            return context.Run(new CompleteWhenOperation(checkCompletion));
        }
    }
}
