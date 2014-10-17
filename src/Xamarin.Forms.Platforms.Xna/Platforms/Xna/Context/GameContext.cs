namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

#if !INTERNAL_CONTEXT
    public
#endif
    class GameContext : IGameContext
    {
        #region Attributes

        readonly List<IGameOperation> _runningOperations;
        readonly Queue<Action<GameTime>> _updateJobs;
        volatile bool haveJobs;
        int _lastOperationIndex;

        #endregion

        #region Constructors

        public GameContext()
        {
            _lastOperationIndex = -1;
            _runningOperations = new List<IGameOperation>();
            _updateJobs = new Queue<Action<GameTime>>();
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (_lastOperationIndex >= 0)
                ContinueOperations(gameTime);

            if (haveJobs)
                RunPendingJobs(gameTime);
        }

        public ContextOperation<T> Run<T>(IGameOperation<T> operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
            return new ContextOperation<T>(operation, this);
        }

        public ContextOperation Run(IGameOperation operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
            return new ContextOperation(operation, this);
        }

        public void Remove(IGameOperation operation)
        {
            _runningOperations.Remove(operation);
        }

        public void Post(Action<GameTime> action)
        {
            lock (_updateJobs)
            {
                _updateJobs.Enqueue(action);
                haveJobs = true;
            }
        }

        public void Post(Action action)
        {
            lock (_updateJobs)
            {
                _updateJobs.Enqueue(gt => action());
                haveJobs = true;
            }
        }

        #endregion

        #region Private Methods
        void ContinueOperations(GameTime gameTime)
        {
            for (int i = _lastOperationIndex; i >= 0; i--)
            {
                if (!_runningOperations[i].Continue(gameTime))
                {
                    _runningOperations.RemoveAt(i);
                    _lastOperationIndex--;
                }
            }
        }

        void RunPendingJobs(GameTime gameTime)
        {
            lock (_updateJobs)
            {
                while (_updateJobs.Count > 0)
                    _updateJobs.Dequeue()(gameTime);
                haveJobs = false;
            }
        }
        #endregion
    }
}
