#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System.Threading.Tasks;

    public static class ContextExtensions
    {
        public static ContextTask Wait(this IGameContext context, Task task)
        {
            return new ContextTask(task, context);
        }

        public static ContextTask<T> Wait<T>(this IGameContext context, Task<T> task)
        {
            return new ContextTask<T>(task, context);
        }

        public static ContextTask<Task<T>> WhenAny<T>(this IGameContext context, params Task<T>[] tasks)
        {
            return context.Wait(Task.WhenAny(tasks));
        }

        public static ContextTask<Task> WhenAny(this IGameContext context, params Task[] tasks)
        {
            return context.Wait(Task.WhenAny(tasks));
        }

        public static ContextTask<T[]> WhenAll<T>(this IGameContext context, params Task<T>[] tasks)
        {
            return context.Wait(Task.WhenAll(tasks));
        }

        public static ContextTask WhenAll(this IGameContext context, params Task[] tasks)
        {
            return context.Wait(Task.WhenAll(tasks));
        }
    }
}
#endif
