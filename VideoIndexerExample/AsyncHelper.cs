using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VideoIndexerExample
{
    /// <summary>
    /// As seen on https://cpratt.co/async-tips-tricks/
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper.MyTaskFactory.StartNew<Task<TResult>>(func).Unwrap<TResult>().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            AsyncHelper.MyTaskFactory.StartNew<Task>(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
