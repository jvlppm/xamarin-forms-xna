#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System;
    using Microsoft.Xna.Framework;

    public class FloatAnimation : GameOperation<TimeSpan>
    {
        #region Attributes

        public readonly TimeSpan Duration;
        public readonly Action<float> ValueStep;
        public readonly float StartValue;
        public readonly float EndValue;
        public readonly Easing Easing;
        #endregion

        #region Properties

        public TimeSpan CurrentDuration { get; private set; }

        #endregion

        #region Constructors

        public FloatAnimation(TimeSpan duration, float startValue, float endValue, Action<float> valueStep, Easing easing = null)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            Duration = duration;

            StartValue = startValue;
            EndValue = endValue;
            ValueStep = valueStep;
            Easing = easing;

            NotifyValue();
        }

        #endregion

        #region Public Methods

        public override bool Continue(GameTime gameTime)
        {
            if (IsCompleted)
                return false;

            CurrentDuration += gameTime.ElapsedGameTime;
            NotifyValue();

            if (CurrentDuration < Duration)
                return true;

            SetResult(CurrentDuration);
            return false;
        }

        #endregion

        #region Private Methods

        void NotifyValue()
        {
            ValueStep(GetValue());
        }

        float GetValue()
        {
            float curDuration = MathHelper.Clamp((float)CurrentDuration.TotalMilliseconds, 0, (float)Duration.TotalMilliseconds);

            var curValue = curDuration / (float)Duration.TotalMilliseconds;
            var lerp = MathHelper.Lerp(StartValue, EndValue, MathHelper.Clamp(curValue, 0, 1));

            if (Easing != null)
                return (float)Easing.Ease(lerp);

            return lerp;
        }

        #endregion
    }
}
#endif
