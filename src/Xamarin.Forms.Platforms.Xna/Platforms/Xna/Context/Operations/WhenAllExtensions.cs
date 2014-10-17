#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System;
    using System.Collections.Generic;

    public static class WhenAllExtensions
    {
        public static ContextOperation WhenAll(this IGameContext context, params ContextOperation[] operations)
        {
            var operation = new DeferredOperation();
            if (operations.Length <= 0)
            {
                operation.SetCompleted();
                return context.Run(operation);
            }

            var remaining = new List<ContextOperation>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;

            foreach (var op in operations)
            {
                op.Operation.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (op.Operation.IsFaulted)
                            errors.Add(op.Operation.Error);
                        else if (op.Operation.IsCanceled)
                            canceled = true;

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors).Flatten());
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetCompleted();
                        }
                    }
                });
            }

            return context.Run(operation);
        }

        public static ContextOperation<T[]> WhenAll<T>(this IGameContext context, params ContextOperation<T>[] operations)
        {
            var operation = new DeferredOperation<T[]>();
            if (operations.Length <= 0)
            {
                operation.SetResult(new T[0]);
                return context.Run(operation);
            }

            var remaining = new List<ContextOperation>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;
            var results = new T[operations.Length];

            int opIndexCount = 0;
            foreach (var op in operations)
            {
                int curOpIndex = opIndexCount;

                op.Operation.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (op.Operation.IsFaulted)
                            errors.Add(op.Operation.Error);
                        else if (op.Operation.IsCanceled)
                            canceled = true;
                        else
                            results[curOpIndex] = ((IGameOperation<T>)op.Operation).GetResult();

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors).Flatten());
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetResult(results);
                        }
                    }
                });

                opIndexCount++;
            }

            return context.Run(operation);
        }
    }
}
#endif
