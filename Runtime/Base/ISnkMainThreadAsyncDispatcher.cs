using System;
using System.Threading.Tasks;

namespace SnkFramework.FluentBinding.Base
{
    public interface ISnkMainThreadAsyncDispatcher
    {
        Task ExecuteOnMainThreadAsync(Action action, bool maskExceptions = true, bool asyncInvoke = true);
        Task ExecuteOnMainThreadAsync(Func<Task> action, bool maskExceptions = true, bool asyncInvoke = true);
        bool RequestMainThreadAction(Action action, bool maskExceptions = true, bool asyncInvoke = true);
        bool IsOnMainThread { get; }
    }
}