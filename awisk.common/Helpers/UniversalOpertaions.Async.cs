using System;
using System.Threading;
using System.Threading.Tasks;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static async Task<T> WithRetryAsync<T>(
            Func<Task<T>> action,
            int retryCount = 3,
            int initialDelayMs = 200,
            double backoff = 2.0,
            CancellationToken ct = default)
        {
            var delay = initialDelayMs;
            for (int attempt = 0; ; attempt++)
            {
                try { return await action().ConfigureAwait(false); }
                catch when (attempt < retryCount)
                {
                    await Task.Delay(delay, ct).ConfigureAwait(false);
                    delay = (int)(delay * backoff);
                }
            }
        }

        public static Task WithRetryAsync(
            Func<Task> action,
            int retryCount = 3,
            int initialDelayMs = 200,
            double backoff = 2.0,
            CancellationToken ct = default)
            => WithRetryAsync(async () => { await action().ConfigureAwait(false); return 0; },
                              retryCount, initialDelayMs, backoff, ct);

        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int milliseconds)
        {
            using var cts = new CancellationTokenSource();
            var delayTask = Task.Delay(milliseconds, cts.Token);
            var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (completed == delayTask) throw new TimeoutException();
            cts.Cancel();
            return await task.ConfigureAwait(false);
        }
    }
}
