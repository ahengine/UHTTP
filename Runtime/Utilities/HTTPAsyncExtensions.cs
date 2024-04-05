using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace UHTTP
{
    public static class HTTPAsyncExtensions
    {
        private const int CancelCheckInterval = 1000;

        /// <summary>
        /// Sends the request asynchornously.
        /// </summary>
        /// <returns>Task to wait for the request.</returns>
        public static Task<UnityWebRequest> SendAsync(this HTTPRequest request)
            => SendAsync(request, CancellationToken.None);

        /// <summary>
        /// Sends the request asynchornously.
        /// </summary>
        /// <param name="cancellation">Token to cancel the request's operation.</param>
        /// <param name="cancelCheckInterval">It will recheck each this milliseconds for cancellation.</param>
        /// <returns>Task to wait for the request.</returns>
        public static Task<UnityWebRequest> SendAsync(this HTTPRequest request, CancellationToken cancellation,
            int cancelCheckInterval = CancelCheckInterval)
        {
            TaskCompletionSource<UnityWebRequest> source = new TaskCompletionSource<UnityWebRequest>();

            request.callback = (webRequest) => FinishTask(source, webRequest);
            request.Send();

            if(cancellation != CancellationToken.None)
                WaitForCancelling(source, request, cancellation, cancelCheckInterval);

            return source.Task;
        }

        private static void FinishTask(TaskCompletionSource<UnityWebRequest> source, UnityWebRequest request)
            => source.TrySetResult(request);

        private static async void WaitForCancelling(TaskCompletionSource<UnityWebRequest> source, HTTPRequest request,
            CancellationToken cancellation, int cancelCheckInterval)
        {
            while (!cancellation.IsCancellationRequested && !source.Task.IsCompleted)
                await Task.Delay(cancelCheckInterval, cancellation);

            if (cancellation.IsCancellationRequested)
            {
                request.webRequest.Abort();
                source.TrySetCanceled(cancellation);
            }
        }
    }
}
