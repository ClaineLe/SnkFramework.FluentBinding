using System;
using System.Threading;
using System.Threading.Tasks;
using SnkFramework.FluentBinding.Base;

namespace SnkFramework.FluentBinding.Base
{
    internal class SnkSnkMainThreadAsyncDispatcher : ISnkMainThreadAsyncDispatcher
    {
        private ISnkBindingLog log = SnkIoCProvider.Instance.Resolve<ISnkBindingLog>();
        public bool IsOnMainThread => _synchronizationContext == SynchronizationContext.Current;

        private SynchronizationContext _synchronizationContext;

        public SnkSnkMainThreadAsyncDispatcher()
        {
            this._synchronizationContext = SynchronizationContext.Current;
        }

        private void exceptionMaskedAction(Action action, bool maskExceptions)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                action();
            }
            catch (Exception exception)
            {
                log.WarnFormat( "Exception thrown when invoking action via dispatcher:{0}", exception);
                if (maskExceptions)
                    log.WarnFormat("Exception masked:{0}", exception);
                else
                    throw;
            }
        }

        public Task ExecuteOnMainThreadAsync(Action action, bool maskExceptions = true, bool asyncInvoke = true)
        {
            if (action == null)
                return Task.CompletedTask;

            var asyncAction = new Func<Task>(() =>
            {
                action();
                return Task.CompletedTask;
            });
            return ExecuteOnMainThreadAsync(asyncAction, maskExceptions, asyncInvoke);
        }

        public async Task ExecuteOnMainThreadAsync(Func<Task> action, bool maskExceptions = true,
            bool asyncInvoke = true)
        {
            if (action == null)
                return;

            var completion = new TaskCompletionSource<bool>();
            var syncAction = new Action(async () =>
            {
                await action();
                completion.SetResult(true);
            });
            RequestMainThreadAction(syncAction, maskExceptions, asyncInvoke);

            // If we're already on main thread, then the action will
            // have already completed at this point, so can just return
            if (completion.Task.IsCompleted)
                return;

            // Make sure we don't introduce weird locking issues  
            // blocking on the completion source by jumping onto
            // a new thread to wait
            await Task.Run(async () => await completion.Task);
        }

        public bool RequestMainThreadAction(Action action, bool maskExceptions = true, bool asyncInvoke = true)
        {
            if (IsOnMainThread)
                exceptionMaskedAction(action, maskExceptions);
            else if (asyncInvoke)
                _synchronizationContext.Post(ignored => { exceptionMaskedAction(action, maskExceptions); }, null);
            else
                _synchronizationContext.Send(ignored => { exceptionMaskedAction(action, maskExceptions); }, null);
            return true;
        }
    }
}