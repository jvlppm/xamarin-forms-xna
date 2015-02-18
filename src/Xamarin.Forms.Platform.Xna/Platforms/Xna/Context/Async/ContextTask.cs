#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System.Threading.Tasks;

    public class ContextTask
    {
        protected readonly Task Task;
        protected readonly IGameContext Context;

        public static explicit operator Task(ContextTask cta)
        {
            return cta.Task;
        }

        public ContextTask(Task task, IGameContext context)
        {
            Task = task;
            Context = context;
        }

        public ContextTaskAwaiter GetAwaiter()
        {
            return new ContextTaskAwaiter(Task, Context);
        }
    }

    public class ContextTask<T> : ContextTask
    {
        public static explicit operator Task<T>(ContextTask<T> cta)
        {
            return (Task<T>)cta.Task;
        }

        public ContextTask(Task<T> task, IGameContext context)
            : base(task, context)
        {
        }

        public new ContextTaskAwaiter<T> GetAwaiter()
        {
            return new ContextTaskAwaiter<T>((Task<T>)Task, Context);
        }
    }
}
#endif
