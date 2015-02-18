namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.ExceptionServices;

#if !INTERNAL_CONTEXT
    public
#endif
    abstract class GameOperation : IGameOperation
    {
        #region Attributes

        Action _waitingForCompletion;

        #endregion

        #region Properties

        public bool IsCompleted { get; private set; }

        public bool IsFaulted { get; private set; }

        public bool IsCanceled { get; private set; }

        public Exception Error { get; protected set; }

        #endregion

        #region Protected Methods

        protected void SetCompleted()
        {
            IsCompleted = true;
            IsCanceled = false;
            IsFaulted = false;
            Error = null;
            NotifyCompletion();
        }

        public virtual void Cancel()
        {
            IsCompleted = true;
            IsCanceled = true;
            IsFaulted = false;
            Error = null;
            NotifyCompletion();
        }

        protected void SetError(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            IsCompleted = true;
            IsCanceled = false;
            IsFaulted = true;
            Error = ex;
            NotifyCompletion();
        }

        #endregion

        #region IAsyncOperation implementation

        public void GetResult()
        {
            if (!IsCompleted)
                throw new InvalidOperationException();
            if (IsFaulted)
                ExceptionDispatchInfo.Capture(Error).Throw();
            if (IsCanceled)
                throw new OperationCanceledException();
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation();
            else
                _waitingForCompletion += continuation;
        }

        #endregion

        #region IOperation implementation

        public abstract bool Continue(GameTime gameTime);

        #endregion

        #region Private Methods

        void NotifyCompletion()
        {
            if (_waitingForCompletion == null)
                return;
            _waitingForCompletion();
            _waitingForCompletion = null;
        }

        #endregion
    }

#if !INTERNAL_CONTEXT
    public
#endif
    abstract class GameOperation<T> : GameOperation, IGameOperation<T>
    {
        T _result;

        protected void SetResult(T result)
        {
            _result = result;
            SetCompleted();
        }

        new public T GetResult()
        {
            base.GetResult();
            return _result;
        }
    }
}
