#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Threading;

    public static class AnimationExtensions
    {
        public static ContextOperation<TimeSpan> Animate(this IGameContext context, TimeSpan duration, float startValue, float endValue, Action<float> valueStep, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            var info = new FloatAnimation(duration, startValue, endValue, valueStep
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
                       );

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }

        public static ContextOperation<TimeSpan> Animate(this IGameContext context, TimeSpan duration, Reference<Color> color, Color endColor, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            if (color == null)
                throw new ArgumentNullException("color");

            return Animate(context, duration, color.Value, endColor, c =>
            {
                color.Value = c;
            }, cancellationToken
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
            );
        }

        public static ContextOperation<TimeSpan> Animate(this IGameContext context, TimeSpan duration, Color startColor, Color endColor, Action<Color> colorStep, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            var info = new FloatAnimation(duration, 0, 1, value =>
            colorStep(Color.Lerp(startColor, endColor, value))
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
                       );

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }
    }
}
#endif
